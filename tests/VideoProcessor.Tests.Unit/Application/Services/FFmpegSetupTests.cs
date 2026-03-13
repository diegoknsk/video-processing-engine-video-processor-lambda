using FluentAssertions;
using VideoProcessor.Application.Services;
using Xabe.FFmpeg;
using Xunit;

namespace VideoProcessor.Tests.Unit.Application.Services;

public class FFmpegSetupTests
{
    [Fact]
    public void IsLambdaEnvironment_WhenAWSLambdaFunctionNameIsSet_ReturnsTrue()
    {
        var original = Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME");
        try
        {
            Environment.SetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME", "my-function");

            FFmpegSetup.IsLambdaEnvironment().Should().BeTrue();
        }
        finally
        {
            Environment.SetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME", original);
        }
    }

    [Fact]
    public void IsLambdaEnvironment_WhenLambdaTaskRootIsSet_ReturnsTrue()
    {
        var origFunc = Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME");
        var origRoot = Environment.GetEnvironmentVariable("LAMBDA_TASK_ROOT");
        try
        {
            Environment.SetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME", null);
            Environment.SetEnvironmentVariable("LAMBDA_TASK_ROOT", "/var/task");

            FFmpegSetup.IsLambdaEnvironment().Should().BeTrue();
        }
        finally
        {
            Environment.SetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME", origFunc);
            Environment.SetEnvironmentVariable("LAMBDA_TASK_ROOT", origRoot);
        }
    }

    [Fact]
    public void IsLambdaEnvironment_WhenNoLambdaEnvVarsSet_ReturnsFalse()
    {
        var origFunc = Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME");
        var origRoot = Environment.GetEnvironmentVariable("LAMBDA_TASK_ROOT");
        try
        {
            Environment.SetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME", null);
            Environment.SetEnvironmentVariable("LAMBDA_TASK_ROOT", null);

            FFmpegSetup.IsLambdaEnvironment().Should().BeFalse();
        }
        finally
        {
            Environment.SetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME", origFunc);
            Environment.SetEnvironmentVariable("LAMBDA_TASK_ROOT", origRoot);
        }
    }

    [Fact]
    public void GetFFmpegPath_WhenExecutablesPathIsConfigured_ReturnsConfiguredPath()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "ffmpeg-test-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(tempDir);
        try
        {
            FFmpeg.SetExecutablesPath(tempDir);

            FFmpegSetup.GetFFmpegPath().Should().Be(tempDir);
        }
        finally
        {
            Directory.Delete(tempDir);
        }
    }

    [Fact]
    public void GetFFmpegPath_WhenExecutablesPathIsNull_ReturnsEmpty()
    {
        var origPath = FFmpeg.ExecutablesPath;
        try
        {
            FFmpeg.SetExecutablesPath(null!);

            FFmpegSetup.GetFFmpegPath().Should().Be(string.Empty);
        }
        finally
        {
            FFmpeg.SetExecutablesPath(origPath!);
        }
    }

    [Fact]
    public async Task EnsureFFmpegInstalledAsync_WhenFFmpegPathAlreadyConfiguredAndDirectoryExists_ReturnsWithoutDownloading()
    {
        // Arrange — aponta para um diretório existente, satisfazendo a condição de early-return
        FFmpeg.SetExecutablesPath(Path.GetTempPath());

        // Act & Assert — deve retornar imediatamente sem tentar download
        var act = () => FFmpegSetup.EnsureFFmpegInstalledAsync();
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureFFmpegInstalledAsync_WhenPathSetButDirNotExists_CallsDownloadWithUserProfilePath()
    {
        // Cobre o caminho não-early-return: path definido mas dir inexistente → IsLambdaEnvironment()=false
        var origFunc = Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME");
        var origRoot = Environment.GetEnvironmentVariable("LAMBDA_TASK_ROOT");
        var origDelegate = FFmpegSetup.DownloadFFmpegToDir;
        var origPath = FFmpeg.ExecutablesPath;

        string? capturedDir = null;
        FFmpegSetup.DownloadFFmpegToDir = dir => { capturedDir = dir; return Task.CompletedTask; };

        var nonExistentPath = Path.Combine(Path.GetTempPath(), "ffmpeg-missing-" + Guid.NewGuid().ToString("N")[..8]);
        FFmpeg.SetExecutablesPath(nonExistentPath);

        Environment.SetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME", null);
        Environment.SetEnvironmentVariable("LAMBDA_TASK_ROOT", null);
        try
        {
            await FFmpegSetup.EnsureFFmpegInstalledAsync();

            var expectedBase = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            capturedDir.Should().Be(Path.Combine(expectedBase, ".ffmpeg"));
        }
        finally
        {
            FFmpegSetup.DownloadFFmpegToDir = origDelegate;
            FFmpeg.SetExecutablesPath(origPath!);
            Environment.SetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME", origFunc);
            Environment.SetEnvironmentVariable("LAMBDA_TASK_ROOT", origRoot);
            if (capturedDir is not null && Directory.Exists(capturedDir))
                Directory.Delete(capturedDir, recursive: true);
        }
    }

    [Fact]
    public async Task EnsureFFmpegInstalledAsync_WhenPathNullAndIsLambdaEnv_CallsDownloadWithTmpPath()
    {
        // Cobre o caminho não-early-return: path nulo → IsLambdaEnvironment()=true (roda apenas em Linux/CI)
        if (!OperatingSystem.IsLinux())
            return; // /tmp hardcoded é válido apenas em Linux; coberto no CI onde o SonarCloud coleta a cobertura

        var origFunc = Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME");
        var origRoot = Environment.GetEnvironmentVariable("LAMBDA_TASK_ROOT");
        var origDelegate = FFmpegSetup.DownloadFFmpegToDir;
        var origPath = FFmpeg.ExecutablesPath;

        string? capturedDir = null;
        FFmpegSetup.DownloadFFmpegToDir = dir => { capturedDir = dir; return Task.CompletedTask; };

        var nonExistentPath = Path.Combine(Path.GetTempPath(), "ffmpeg-missing-" + Guid.NewGuid().ToString("N")[..8]);
        FFmpeg.SetExecutablesPath(nonExistentPath);

        Environment.SetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME", "test-function");
        Environment.SetEnvironmentVariable("LAMBDA_TASK_ROOT", null);
        try
        {
            await FFmpegSetup.EnsureFFmpegInstalledAsync();

            capturedDir.Should().Be("/tmp/.ffmpeg");
        }
        finally
        {
            FFmpegSetup.DownloadFFmpegToDir = origDelegate;
            FFmpeg.SetExecutablesPath(origPath!);
            Environment.SetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME", origFunc);
            Environment.SetEnvironmentVariable("LAMBDA_TASK_ROOT", origRoot);
            if (capturedDir is not null && Directory.Exists(capturedDir))
                Directory.Delete(capturedDir, recursive: true);
        }
    }
}

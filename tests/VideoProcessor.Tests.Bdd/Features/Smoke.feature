Feature: Smoke Test
  Projeto BDD está configurado

Scenario: Projeto BDD está configurado
  Given o projeto de testes BDD existe
  When eu executo os testes
  Then o teste deve passar

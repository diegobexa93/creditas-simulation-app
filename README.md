#Loan Simulation API → RabbitMQ → Consumers → MongoDB

Este teste de ponta a ponta (E2E) cobre o fluxo completo da aplicação, desde o recebimento de uma requisição HTTP na API até a persistência do resultado da simulação no MongoDB, passando por RabbitMQ e os consumers reais.

---

## ✅ Fluxo Validado no Teste `LoanSimulationApiE2ETests`

```
POST /CreateBatch (API Controller)
        ↓
Publica LoanSimulationGenerateEvent no RabbitMQ
        ↓
Consumer 1: CreateLoanSimulationConsumer
 - Simula o empréstimo
 - Publica dois eventos:
     - LoanSimulationEmailEvent
     - LoanSimulationPersistenceEvent
        ↓
Consumer 2: LoanSimulationPersistenceConsumer
 - Persiste a simulação no MongoDB
        ↓
Consulta do MongoDB no teste (via Repository)
```

---

## 📦 Estrutura do Projeto e Decisões de Arquitetura

A aplicação segue uma arquitetura **desacoplada, assíncrona e orientada a eventos**, organizada em múltiplos serviços especializados. Os principais componentes são:

- **🔀 API Gateway (`ApiGatewayYarp`)**  
  Atua como ponto de entrada para o frontend, realizando roteamento e balanceamento de carga para os serviços internos.

- **📡 API (`CreditSimulator.API`)**  
  Responsável por expor endpoints HTTP públicos e publicar eventos no RabbitMQ com base nas requisições recebidas.

- **⚙️ Worker (`CreditSimulatorService.Worker`)**  
  Consome eventos do RabbitMQ, executa a lógica de simulação de crédito e publica novos eventos ou persiste os dados no MongoDB.

- **📨 RabbitMQ**  
  Barramento de mensagens que desacopla os serviços e viabiliza comunicação assíncrona, confiável e escalável.

- **🗂️ MongoDB**  
  Banco de dados NoSQL utilizado para armazenar os resultados das simulações de forma estruturada.

- **🧪 Testes End-to-End**  
  Validação do fluxo completo (API → RabbitMQ → Workers → MongoDB), utilizando infraestrutura real com Testcontainers.

- **🧩 Testes de Integração**  
  Validação de módulos individuais (como Consumers e Repositórios), utilizando RabbitMQ e MongoDB reais para simular o ambiente de produção.

- **🧠 Testes Unitários**  
  Validação isolada de regras de negócio (por exemplo, o cálculo da simulação) com dados controlados e mocks.

- **📊 Testes de Benchmark**  
  Avaliam a performance do motor de simulação de crédito, permitindo medições comparativas sob diferentes cenários.

---

### 🧭 Decisões de Arquitetura

- Separação de responsabilidades entre API (entrada) e Worker (processamento assíncrono)
- Uso de eventos para garantir desacoplamento e escalabilidade
- Aplicação do padrão CQRS e princípios SOLID nos serviços
- Comunicação assíncrona baseada em mensageria com `MassTransit` + `RabbitMQ`
- Persistência orientada a documentos com `MongoDB.Driver`
- Camada de testes estruturada por tipo (unitários, integração e E2E), com uso intensivo de `Testcontainers`
---



## 🛠️ Instruções de Setup

### Requisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Docker](https://www.docker.com/)

### Restaurar dependências e compilar

```bash
cd src/Creditas.Simulation

# Restaurar pacotes
dotnet restore

# Compilar solução
dotnet build
```

### Rodar os testes End-to-End

```bash
cd tests/CreditSimulator.EndToEndTests

dotnet test
```

O teste irá:
- Iniciar containers do RabbitMQ e MongoDB
- Levantar a API real em memória
- Instanciar os consumers reais diretamente no teste
- Simular uma requisição real na API
- Verificar o dado persistido no MongoDB

---

## 📬 Exemplos de Requisições para os Endpoints

### POST `/api/LoanSimulation/CreateBatch`

```http
POST /api/LoanSimulation/CreateBatch
Content-Type: application/json

{
  "simulations": [
    {
      "email": "cliente@teste.com",
      "valueLoan": 10000,
      "paymentTerm": 12,
      "birthDate": "2000-01-01"
    },
    {
      "email": "outra@cliente.com",
      "valueLoan": 20000,
      "paymentTerm": 24,
      "birthDate": "1990-05-20"
    }
  ]
}
```

### Retorno esperado:
```json
"8c9c2764-8d1b-4d1f-902a-05fa948b3c44"
```
(*ID do batch que foi publicado*)

---


## 💡 Como o teste está estruturado

### `InitializeAsync()`:
- Sobe o container do RabbitMQ
- Sobe o container do MongoDB
- Instancia os dois consumers reais:
  - `CreateLoanSimulationConsumer`
  - `LoanSimulationPersistenceConsumer`
- Registra os endpoints do MassTransit
- Sobe a API real com configurações injetadas via `WebApplicationFactory`

### `ShouldSimulateAndPersistLoanInMongo_ViaApi()`:
- Faz requisição HTTP POST para a rota `/CreateBatch`
- Aguarda até que o resultado apareça no MongoDB (com polling de 10s)
- Valida que os dados foram realmente persistidos

---

## 🚀 Sugestões para Evoluir

- ✅ Validar e implementar o envio do evento de e-mail (`LoanSimulationEmailEvent`)
- ✅ Rodar o mesmo fluxo com Docker Compose para CI/CD
- 🔄 Testar múltiplas simulações por batch
- ⚠️ Simular falhas no Mongo ou no RabbitMQ para testar resiliência
- 📈 Medir tempo de resposta entre publicação e persistência

---

> Este teste garante que a integração completa entre módulos assíncronos esteja funcionando corretamente e que o sistema como um todo está reagindo conforme esperado.


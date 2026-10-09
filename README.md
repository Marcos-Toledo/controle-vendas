# Controle de Vendas

Sistema web para controle de vendas: cadastro de **clientes**, **produtos** e
lançamento de **vendas** com múltiplos itens.

Desenvolvido como teste prático — requisitos no escopo do desafio.

## Stack

| Camada    | Tecnologia                                              |
| --------- | ------------------------------------------------------- |
| Frontend  | React 19 + TypeScript + Vite + Tailwind CSS + Router    |
| Backend   | .NET 8 (ASP.NET Core, C#) + Entity Framework Core 8     |
| Banco     | PostgreSQL 16 (SQL Server/MySQL seriam equivalentes)    |
| Testes    | xUnit                                                   |
| API Docs  | Swagger / OpenAPI (Swashbuckle)                         |

> **Por que PostgreSQL?** O escopo permite SQL Server (preferencial), PostgreSQL
> ou MySQL. Escolhi PostgreSQL por já rodar em container Docker no ambiente de
> desenvolvimento, sem licença e multiplataforma. Todo o acesso a dados passa
> pelo EF Core, então trocar o provider (Npgsql → SqlServer) é questão de uma
> linha em `Program.cs` + ajuste de sintaxe no script SQL.

## Arquitetura

Backend em camadas seguindo Clean Architecture e SOLID:

```
backend/
├── ControleVendas.sln
├── src/
│   ├── ControleVendas.Domain/          # entidades e regras de negócio puras
│   │   └── Entities/                   #   Cliente, Produto, Venda, ItemVenda
│   ├── ControleVendas.Application/     # casos de uso, DTOs, validações
│   │   ├── DTOs/
│   │   ├── Interfaces/                 #   portas (repositórios, IUnitOfWork)
│   │   └── Services/
│   ├── ControleVendas.Infrastructure/  # EF Core, DbContext, repositórios
│   │   ├── Persistence/Configurations/ #   mapeamentos Fluent API
│   │   ├── Persistence/Migrations/     #   InitialCreate (paridade com o script)
│   │   └── Repositories/
│   └── ControleVendas.Api/             # controllers, middlewares, Swagger
└── tests/
    └── ControleVendas.Tests/           # xUnit — regras de negócio
```

Direção das dependências: `Api → Application → Domain` e
`Api → Infrastructure → Application`. O domínio não conhece EF Core nem HTTP.

## Banco de dados

Modelo normalizado (3FN) com chaves primárias, estrangeiras, índices e
constraints de obrigatoriedade — script completo em
[`database/01_create_schema.sql`](database/01_create_schema.sql).

```
clientes ──────< vendas ──────< itens_venda >────── produtos
```

Decisões de modelagem:

- **`clientes.cpf` com índice único** — a validação de duplicidade exigida no
  escopo existe no banco, não só na aplicação. O índice também acelera a busca
  por CPF na tela de vendas.
- **`produtos.nome` com índice único** — mesma regra de duplicidade.
- **`itens_venda.preco_unitario` gravado no item** — preserva o preço do
  momento da venda; mudanças futuras no cadastro do produto não alteram o
  histórico.
- **`itens_venda.total_item` coluna gerada** (`quantidade * preco_unitario`) —
  total por item garantido pelo banco.
- **`vendas.valor_total` desnormalizado de forma controlada** — calculado pela
  aplicação na gravação, agiliza listagens; os itens continuam a fonte da
  verdade.
- **`ON DELETE RESTRICT` em cliente/produto** usados em vendas — preserva a
  integridade do histórico; `CASCADE` apenas nos itens da própria venda.
- **CHECKs** de CPF (11 dígitos), quantidade > 0 e valores ≥ 0.

## Como rodar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (ou superior — o projeto
  mira `net8.0`)
- [Node.js 20+](https://nodejs.org) e [pnpm](https://pnpm.io) (`npm` também
  funciona, trocando `pnpm` por `npm` nos comandos)
- [Docker](https://www.docker.com) para o PostgreSQL — ou uma instância própria

### 1. Banco de dados

Com Docker (recomendado — sobe o Postgres e já executa o script de criação):

```bash
docker compose up -d
```

Isso sobe o PostgreSQL na porta **5433** e executa
`database/01_create_schema.sql` automaticamente.

Alternativa sem Docker: com um PostgreSQL rodando, execute o script manualmente:

```bash
psql -U postgres -f database/01_create_schema.sql
```

Também há uma migration EF (`InitialCreate`) com paridade total ao script —
num banco **vazio**, `dotnet ef database update` gera o mesmo schema
(requer `dotnet tool restore` no diretório `backend/`). Use **ou** o script
**ou** a migration, nunca os dois.

Ajuste a connection string em
`backend/src/ControleVendas.Api/appsettings.json` se necessário.

### 2. Backend

```bash
cd backend
dotnet restore
dotnet run --project src/ControleVendas.Api
```

API em `http://localhost:5125` — Swagger UI em
`http://localhost:5125/swagger`.

### 3. Frontend

```bash
cd frontend
pnpm install
pnpm dev
```

App em `http://localhost:5173` — as chamadas `/api/*` são redirecionadas à API
pelo proxy do Vite.

### 4. Testes

```bash
cd backend
dotnet test
```

## API

| Método | Rota                          | Descrição                                |
| ------ | ----------------------------- | ---------------------------------------- |
| GET    | `/api/clientes`               | Lista clientes                           |
| GET    | `/api/clientes/buscar?termo=` | Busca por CPF ou nome (tela de vendas)   |
| POST   | `/api/clientes`               | Cadastra (409 se CPF duplicado)          |
| PUT    | `/api/clientes/{id}`          | Atualiza                                 |
| DELETE | `/api/clientes/{id}`          | Remove (restrito se houver vendas)       |
| GET    | `/api/produtos`               | Lista produtos                           |
| POST   | `/api/produtos`               | Cadastra (409 se nome duplicado)         |
| PUT    | `/api/produtos/{id}`          | Atualiza                                 |
| DELETE | `/api/produtos/{id}`          | Remove                                   |
| GET    | `/api/vendas`                 | Lista vendas com itens                   |
| POST   | `/api/vendas`                 | Registra venda (calcula total no server) |

Erros de validação retornam `400`/`409` com mensagens claras em pt-BR.

## Validações e regras de negócio

- CPF: 11 dígitos, dígito verificador válido, único no banco
- Cliente: nome obrigatório; e-mail validado quando informado
- Produto: nome único; valor unitário ≥ 0
- Venda: data sugere o dia atual; pelo menos 1 item; quantidade > 0; preço
  unitário capturado do produto no momento do lançamento; total calculado no
  backend (o frontend exibe, mas não define)
- Conflitos de unicidade do banco são traduzidos em mensagens amigáveis
  (ex.: "CPF já cadastrado")

## Checklist do escopo

- [x] Cadastro de clientes (CPF único, nome obrigatório, nascimento, e-mail,
      telefone)
- [x] Cadastro de produtos (nome único, valor unitário)
- [x] Tela de vendas com seleção de cliente por CPF/nome, múltiplos itens e
      totalizadores
- [x] React + .NET 8 + PostgreSQL via EF Core
- [x] PKs, FKs, índices, NOT NULL, normalização
- [x] Script SQL completo de criação do banco
- [x] Clean Architecture / SOLID (diferencial)
- [x] Swagger/OpenAPI (diferencial)
- [x] Testes automatizados nas regras de negócio (diferencial)
- [x] Documentação das decisões — este README (diferencial)

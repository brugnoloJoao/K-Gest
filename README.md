<h1 align="center">
  <br>
  <img src="https://img.shields.io/badge/K--Gest-Sistema%20de%20Gest%C3%A3o-blue?style=for-the-badge" alt="K-Gest">
  <br>
  K-Gest
  <br>
</h1>

<h4 align="center">Sistema de Gestão de Estoque e Produção de Cozinha Comercial desenvolvido para a <strong>Kantine</strong>.</h4>

<p align="center">
  <img alt="C#" src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=csharp&logoColor=white"/>
  <img alt=".NET 10" src="https://img.shields.io/badge/.NET%2010-512BD4?style=flat-square&logo=dotnet&logoColor=white"/>
  <img alt="ASP.NET Core MVC" src="https://img.shields.io/badge/ASP.NET%20Core%20MVC-512BD4?style=flat-square"/>
  <img alt="SQL Server" src="https://img.shields.io/badge/SQL%20Server-CC2927?style=flat-square&logo=microsoft-sql-server&logoColor=white"/>
  <img alt="Bootstrap" src="https://img.shields.io/badge/Bootstrap-7952B3?style=flat-square&logo=bootstrap&logoColor=white"/>
  <img alt="Status" src="https://img.shields.io/badge/status-em%20desenvolvimento-yellow?style=flat-square"/>
</p>

<p align="center">
  <a href="#sobre">Sobre</a> •
  <a href="#o-problema">O Problema</a> •
  <a href="#funcionalidades">Funcionalidades</a> •
  <a href="#tecnologias">Tecnologias</a> •
  <a href="#como-executar">Como Executar</a> •
  <a href="#arquitetura-e-estrutura">Estrutura</a> •
  <a href="#equipe">Equipe</a>
</p>

## 📋 Sobre <a name="sobre"></a>
O **K-Gest** é uma solução original projetada para atuar como uma camada de otimização operacional no ambiente de cozinha profissional da confeitaria e restaurante **Kantine**. Desenvolvido como Projeto Integrador Interdisciplinar 1 no CST em Análise e Desenvolvimento de Sistemas (SENAI Ribeirão Preto), o sistema atua como uma ferramenta de suporte à tomada de decisão, focando no planejamento e controle da produção (PCP), redução de desperdícios e eficiência de suprimentos.

O projeto adota uma metodologia de desenvolvimento ágil híbrida, unindo a visibilidade do **Kanban**, a dinâmica incremental por sprints do **Scrum** e a busca por melhoria contínua (**Lean/Kaizen**).

## ⚠️ O Problema <a name="o-problema"></a>
Em cozinhas comerciais, a falta de controles rígidos impacta diretamente o Custo de Mercadoria Vendida (CMV). A operação parceira **Kantine** enfrentava desafios como:
* Falta de padronização operacional (ausência de POPs e Fichas Técnicas consolidadas).
* Controle de estoque manual e pouco confiável, gerando furos e compras emergenciais.
* Ausência de uma rotina automatizada de entradas e saídas que considere o lote e a validade das mercadorias (**PEPS/FIFO**).

O **K-Gest** foi concebido para mitigar essas falhas, trazendo automação virtual para alinhar o estoque físico com o virtual.

## ✨ Funcionalidades Planejadas e Prontas <a name="funcionalidades"></a>
O escopo do sistema baseia-se nos seguintes macro-módulos:

- [x] **Controle de Acesso:** Autenticação segura de usuários por meio de login e senha (RF-01).
- [x] **Gestão de Inventário (CRUD):** Cadastro e manutenção completa de insumos, contendo informações básicas de controle (RF-09).
- [x] **Controle de Movimentações:** Registro simplificado de entradas e saídas de insumos do estoque físico (RF-07, RF-08).
- [x] **Rastreabilidade de Lotes:** Registro de lotes de insumos armazenando quantidade, data de entrada e validade (RF-13).
- [x] **Fichas Técnicas / Receitas:** Cadastro de receitas associando ingredientes, quantidades específicas e unidades de medida (RF-10, RF-11).
- [x] **Gestão Preditiva (Lista de Compras):** Geração automatizada de uma lista de compras baseada no histórico de vendas de pratos, estoque atual e proximidade de vencimento dos lotes (RF-06).
- [x] **Monitoramento de Performance (BI / Dashboards):** Painéis analíticos para visualização de indicadores-chave de consumo e mensuração de margens de desperdício (RF-02).
- [ ] **Exportação de Dados:** Filtros customizados e exportação/impressão de relatórios e listas de compras no formato XLSX/Excel (RF-04, RF-05).

## 🛠️ Tecnologias e Ferramentas <a name="tecnologias"></a>
Configurado sob o ecossistema estável da Microsoft, o projeto foi arquitetado com:

* **Ambiente Backend:** C# executado sobre a plataforma **.NET 10** e o framework **ASP.NET Core MVC** (padrão Model-View-Controller para separação clara de responsabilidades).
* **Processamento em Segundo Plano:** Uso da classe nativa `BackgroundService` hospedada para rodar tarefas pesadas em threads separadas, mantendo a UI responsiva.
* **Persistência de Dados:** Banco de dados relacional **Microsoft SQL Server**, utilizando a estratégia arquitetural **DAO (Data Access Object)** para isolar totalmente as instruções SQL da lógica das controllers.
* **Interface Frontend:** Estruturação em **HTML5 Semântico**, estilização com **CSS3**, interatividade dinâmica com **JavaScript** e responsividade fluida via **Bootstrap**.
* **Modelagem de Dados:** Desenhado inicialmente com a ferramenta **brModelo**.
* **IDE de Desenvolvimento:** Visual Studio 2026.

## 🗄️ Modelagem do Banco de Dados <a name="banco-de-dados"></a>

O sistema utiliza o banco de dados relacional **Microsoft SQL Server**. Abaixo estão mapeadas as principais entidades estruturadas no dicionário de dados do projeto.

<details>
<summary><b>📐 Clique aqui para visualizar o Dicionário de Tabelas</b></summary>
<br>

### 1. Tabela: `Usuarios`
Armazena as credenciais e informações de acesso ao sistema (RF-01).

| Campo | Tipo | Restrições | Descrição |
| :--- | :--- | :--- | :--- |
| `id_usuario` | INT | PK, Identity | Identificador único do usuário. |
| `nome` | VARCHAR(100) | NOT NULL | Nome completo do colaborador. |
| `email` | VARCHAR(100) | NOT NULL, UNIQUE | E-mail utilizado para login. |
| `senha` | VARCHAR(255) | NOT NULL | Hash seguro da senha de acesso. |

### 2. Tabela: `Insumos`
Registra os ingredientes e materiais cadastrados no inventário (RF-09).

| Campo | Tipo | Restrições | Descrição |
| :--- | :--- | :--- | :--- |
| `id_insumo` | INT | PK, Identity | Identificador único do insumo. |
| `nome` | VARCHAR(100) | NOT NULL | Nome do ingrediente (ex: Farinha de Trigo). |
| `unidade_medida` | VARCHAR(10) | NOT NULL | Unidade de consumo (KG, G, L, ML, UN). |
| `estoque_minimo` | DECIMAL(10,2)| NOT NULL | Limite mínimo para alerta de reposição. |

### 3. Tabela: `Lotes`
Controla a rastreabilidade, quantidades físicas atuais e validades (PEPS/FIFO) (RF-13).

| Campo | Tipo | Restrições | Descrição |
| :--- | :--- | :--- | :--- |
| `id_lote` | INT | PK, Identity | Identificador único do lote. |
| `id_insumo` | INT | FK (`Insumos`) | Associação ao insumo correspondente. |
| `codigo_lote` | VARCHAR(50)  | NOT NULL | Código de identificação do fabricante/registro. |
| `quantidade` | DECIMAL(10,2)| NOT NULL | Quantidade atual disponível neste lote. |
| `data_entrada` | DATETIME | NOT NULL | Data em que o insumo deu entrada na cozinha. |
| `data_validade` | DATETIME | NOT NULL | Data de vencimento do lote. |

### 4. Tabela: `Receitas` (Fichas Técnicas)
Estrutura as preparações da confeitaria/cozinha (RF-10).

| Campo | Tipo | Restrições | Descrição |
| :--- | :--- | :--- | :--- |
| `id_receita` | INT | PK, Identity | Identificador único da receita/prato. |
| `nome_prato` | VARCHAR(100) | NOT NULL | Nome do produto final (ex: Bolo de Cenoura). |
| `rendimento` | INT | NOT NULL | Quantidade de porções padronizadas geradas. |

### 5. Tabela: `Itens_Receita`
Tabela associativa que compõe os ingredientes de cada ficha técnica (RF-11).

| Campo | Tipo | Restrições | Descrição |
| :--- | :--- | :--- | :--- |
| `id_receita` | INT | PK, FK (`Receitas`) | Associação à receita pai. |
| `id_insumo` | INT | PK, FK (`Insumos`) | Associação ao ingrediente necessário. |
| `quantidade_necessaria`| DECIMAL(10,2)| NOT NULL| Proporção exata utilizada na preparação. |

</details>

## 🚀 Como Executar <a name="como-executar"></a>

### Pré-requisitos
Antes de começar, certifique-se de ter instalado em sua máquina:
* [Visual Studio (2022 ou superior)](https://visualstudio.microsoft.com/) com a carga de trabalho de **Desenvolvimento Web e ASP.NET** instalada.
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* [Microsoft SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) (ou LocalDB configurado)
* Git

### Passo a passo
```bash
# Clone o repositório
git clone https://github.com/brugnoloJoao/K-Gest.git

# Acesse a pasta do projeto
cd K-Gest
```

1. Abra o arquivo **`K-Gest.sln`** no Visual Studio.
2. Aguarde o carregamento das dependências do projeto.
3. Pressione **`Ctrl + F5`** (ou clique em **Iniciar sem Depuração**) para executar.
4. O navegador abrirá automaticamente com a aplicação em execução.

> **Dica:** caso ocorra algum erro de dependência, clique com o botão direito na solução e selecione **"Restaurar Pacotes NuGet"**.

---

## 📁 Estrutura do Projeto
O projeto segue o padrão arquitetural clássico do ASP.NET MVC acoplado ao isolamento por camada de persistência:
```
K-Gest/
│
├── K-Gest/                   # Pasta principal do projeto
│   ├── Controllers/          # Controladores (lógica de negócio)
│   ├── Models/               # Modelos de dados
│   ├── Views/                # Páginas HTML/Razor
│   └── wwwroot/              # Arquivos estáticos (CSS, JS, imagens)
│
├── K-Gest.sln                # Solução do Visual Studio
└── .gitignore
```

---

## 👥 Equipe
Projeto desenvolvido pelos acadêmicos do SENAI Ribeirão Preto:

Orientador: Prof. MSc. Gustavo Martins Nunes Avellar ⚖️

<table>
  <tr>
    <td align="center">
      <a href="https://github.com/brugnoloJoao">
        <img src="https://github.com/brugnoloJoao.png" width="100px;" alt="Foto do João Pedro Alves Brugnolo" style="border-radius:50%;"/><br>
        <sub><b>João Pedro Alves Brugnolo</b></sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/lucasbrocha">
        <img src="https://github.com/Luca4sR0cha.png" width="100px;" alt="Foto do Lucas Bernardes da Rocha" style="border-radius:50%;"/><br>
        <sub><b>Lucas Bernardes da Rocha</b></sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/raquelcpg">
        <img src="https://github.com/RaquelCastilheiro.png" width="100px;" alt="Foto da Raquel Castilheiro Palma Gonçalves" style="border-radius:50%;"/><br>
        <sub><b>Raquel C. P. Gonçalves</b></sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/SamaraVitoria">
        <img src="https://github.com/vtoriaoliv.png" width="100px;" alt="Foto da Samara Vitória Santos Oliveira" style="border-radius:50%;"/><br>
        <sub><b>Samara V. S. Oliveira</b></sub>
      </a>
    </td>
  </tr>
</table>

---

## 📄 Licença

Este software foi desenvolvido exclusivamente para fins acadêmicos como Projeto Integrador Interdisciplinar do Curso Superior de Tecnologia em Análise e Desenvolvimento de Sistemas do SENAI Ribeirão Preto.

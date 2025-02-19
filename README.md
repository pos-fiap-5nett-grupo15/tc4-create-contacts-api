# Tech Challenge 4 - Grupo 15

Projeto realizado pelo **Grupo 15** da turma da FIAP de Arquitetura de Sistemas .NET com Azure


## Autores

||
|--|
| Guilherme Castro Batista Pereira |
| Caio Vinícius Moura Santos Maia |
| Evandro Prates Silva |
| Luis Gustavo Gonçalves Reimberg |


## CreateContact

### Tecnologias Utilizadas
- .NET 8
- Dapper
- RabbitMQ
- FluentValidation
- XUnit
- MediatR
- Moq

Dentro da arquitetura de microsserviços desenvolvida para este tech challenge, este projeto realiza a função de criar os contatos, seguindo o passo a passo abaixo:

### API
- Receber a requisição
- Validar os dados da requisição e garante que o contato já não exista na base de dados
- Cria um pré-contato com status inicial para a fila
- Enviar a requisição para a respectiva fila de criação

### Worker
- Consumir a fila de criações
- Realizar a validação adicional para garantir a integridade do contato/registro no banco de dados
- Cria o contato e atualiza seu status

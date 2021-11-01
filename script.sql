create database pedido_hamburger;

CREATE TABLE clientes (
	codigo int2 NULL,
	nome varchar(255) NULL,
	endereco varchar(255) NULL,
	telefone varchar(20) NULL
);

INSERT INTO clientes
(codigo, nome, endereco, telefone)
VALUES(2, 'Jaqueline', 'Rua 1234', '(11)98999-9999');

SELECT *
FROM clientes;

CREATE TABLE ingredientes (
	codigo int not null,
	nome varchar(255) not NULL
);

INSERT INTO ingredientes
(codigo, nome)
VALUES(1, 'Pão');

INSERT INTO ingredientes
(codigo, nome)
VALUES(2, 'Carne');

INSERT INTO ingredientes
(codigo, nome)
VALUES(3, 'Alface');

INSERT INTO ingredientes
(codigo, nome)
VALUES(4, 'Tomate');

INSERT INTO ingredientes
(codigo, nome)
VALUES(5, 'Molho especial');

select * from ingredientes;

CREATE TABLE hamburgeres (
	codigo int not null,
	nome varchar(255) not NULL,
	valor float not NULL
);


CREATE TABLE hamburgeres_ingredientes (
	codigo_hamburger int not null,
	codigo_ingrediente int not null
);


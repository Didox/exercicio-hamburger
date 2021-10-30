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

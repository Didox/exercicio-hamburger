CREATE TABLE public.clientes (
	codigo int2 NULL,
	nome varchar(255) NULL,
	endereco varchar(255) NULL,
	telefone varchar(20) NULL
);


SELECT *
FROM clientes;


INSERT INTO clientes
(codigo, nome, endereco, telefone)
VALUES(2, 'Jaqueline', 'Rua 1234', '(11)98999-9999');

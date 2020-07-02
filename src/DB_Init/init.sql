DROP TABLE IF EXISTS `Books`;
CREATE TABLE `Books` (
  `Id` char(36) NOT NULL,  
  `Title` varchar(45) NOT NULL,  
  `Category` varchar(45) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

LOCK TABLES `Books` WRITE;
INSERT INTO `Books` VALUES 
('51b5fe6b-aaba-4217-9deb-8d741a4d6a72', 'Designing Data Intensive Applications', 'Software Engineering'),
('d02d9c93-42c5-4258-b936-91612261eeb0', 'Kubernetes_in_Action', 'Software Engineering'),
('94fa2b30-7c57-4e8c-9953-66ea8683a485', 'Microservice Patterns', 'Software Engineering'),
('e2edafde-3943-4edc-9165-2553901bb7bb', 'Collective Intelligence in Action', 'Internet'),
('7887adef-9df2-467d-ab31-2da60e4caa34', 'Hello! Flex 4', 'Internet'),
('abfb322a-8912-407e-9475-4e26ed6cea2a', 'MongoDB in Action', 'Database'),
('4b7e1b0c-2133-4b13-b4fb-9bc08ca41ce6', 'Mastering SQL Server', 'Database'),
('3dcf621c-583a-45a6-ba1e-f56682082e23', 'Hibernate in Action', 'Database');
UNLOCK TABLES;
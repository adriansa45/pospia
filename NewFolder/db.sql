CREATE TABLE `order_lines` (
  `OrderLineId` int NOT NULL AUTO_INCREMENT,
  `OrderId` int DEFAULT NULL,
  `ProductId` int DEFAULT NULL,
  `amount` int DEFAULT NULL,
  `price` decimal(18,4) DEFAULT NULL,
  `total` decimal(18,4) DEFAULT NULL,
  `created` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`OrderLineId`)
);

CREATE TABLE `orders` (
  `OrderId` int NOT NULL AUTO_INCREMENT,
  `total` decimal(18,4) DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `created` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`OrderId`)
);

CREATE TABLE `products` (
  `ProductId` int NOT NULL AUTO_INCREMENT,
  `name` varchar(256) DEFAULT NULL,
  `amount` int DEFAULT NULL,
  `price` decimal(18,4) DEFAULT NULL,
  `image` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`ProductId`)
);

CREATE TABLE `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(256) DEFAULT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `NormalizedEmail` varchar(256) DEFAULT NULL,
  `Password` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`id`)
);

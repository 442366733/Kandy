CREATE database if NOT EXISTS `ddd_test` default character set utf8mb4 collate utf8mb4_unicode_ci;
use `ddd_test`;

SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS `t_order` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `order_code` varchar(50) DEFAULT NULL COMMENT '订单编号',
  `order_amount` decimal NOT NULL DEFAULT '0' COMMENT '订单金额',
  `customer_id` int(11) NOT NULL DEFAULT '0' COMMENT '客户id',
  `address` varchar(200) NOT NULL DEFAULT '' COMMENT '地址',
  `create_user_id` int(11)  NULL COMMENT '创建用户',
  `create_time` datetime  NULL  COMMENT '创建时间',
  `update_user_id` int(11) NULL COMMENT '更新用户',
  `update_time` datetime  NULL  COMMENT '更新时间',
  `deleted` int NOT NULL DEFAULT '0' COMMENT '是否删除',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


CREATE TABLE IF NOT EXISTS  `t_order_item` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `order_id` varchar(50) DEFAULT NULL COMMENT '订单编号',
  `sku_code` varchar(50) NOT NULL DEFAULT '' COMMENT 'sku编号',
  `sku_number` int(11) NOT NULL DEFAULT '0' COMMENT 'sku数量',
	`create_user_id` int(11)  NULL COMMENT '创建用户',
  `create_time` datetime  NULL  COMMENT '创建时间',
  `update_user_id` int(11) NULL COMMENT '更新用户',
  `update_time` datetime  NULL  COMMENT '更新时间',
  `deleted` int NOT NULL DEFAULT '0' COMMENT '是否删除',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
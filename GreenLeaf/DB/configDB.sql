-- phpMyAdmin SQL Dump
-- version 4.8.5
-- https://www.phpmyadmin.net/
--
-- Хост: localhost
-- Время создания: Ноя 01 2019 г., 05:47
-- Версия сервера: 8.0.13-4
-- Версия PHP: 7.2.24-0ubuntu0.18.04.1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `wLhC3u9El4`
--

-- --------------------------------------------------------

--
-- Структура таблицы `ACCOUNT`
--

CREATE TABLE `ACCOUNT` (
  `ID` int(11) NOT NULL COMMENT 'идентификатор',
  `CODE` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT 'код пользователя',
  `LOGIN` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT 'логин пользователя',
  `PASSWORD` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL COMMENT 'пароль пользователя (зашифрованный)',
  `SURNAME` varchar(50) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT 'фамилия пользователя',
  `NAME` varchar(50) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT 'имя пользователя',
  `PATRONYMIC` varchar(50) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT 'отчество пользователя',
  `ADRESS` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT 'адрес пользователя (зашифрованный)',
  `PHONE` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT 'телефон пользователя (зашифрованный)',
  `SEX` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'пол пользователя',
  `PURCHASE_INVOICE` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак создания приходных накладных',
  `SALES_INVOICE` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак создания расходных накладных',
  `REPORTS` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак доступа к отчетам',
  `REPORT_PURCHASE_INVOICE` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак доступа к отчетам по чужим приходным накладным',
  `REPORT_UN_ISSUE_PURCHASE_INVOICE` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак отмены проведения приходных накладных',
  `REPORT_SALES_INVOICE` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак доступа к отчетам по чужим расходным накладным',
  `REPORT_UN_ISSUE_SALES_INVOICE` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак отмены проведения расходных накладных',
  `REPORT_INCOME_EXPENSE` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак доступа к отчету по балансу',
  `COUNTERPARTY` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак доступа к контрагентам',
  `COUNTERPARTY_PROVIDER` tinyint(1) DEFAULT '0' COMMENT 'признак доступа к поставщикам',
  `COUNTERPARTY_PROVIDER_ADD` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак добавления поставщиков',
  `COUNTERPARTY_PROVIDER_EDIT` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак редактирования поставщиков',
  `COUNTERPARTY_PROVIDER_DELETE` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак аннулирования поставщиков',
  `COUNTERPARTY_CUSTOMER` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак доступа к покупателям',
  `COUNTERPARTY_CUSTOMER_ADD` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак добавления покупателей',
  `COUNTERPARTY_CUSTOMER_EDIT` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак редактирования покупателей',
  `COUNTERPARTY_CUSTOMER_DELETE` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак аннулирования покупателей',
  `WAREHOUSE` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак доступа к управлению складом',
  `WAREHOUSE_ADD_PRODUCT` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак добавления вида товара',
  `WAREHOUSE_EDIT_PRODUCT` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак редактирования вида товара',
  `WAREHOUSE_ANNULATE_PRODUCT` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак аннулирования вида товара',
  `WAREHOUSE_EDIT_COUNT` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак редактирования количества товара',
  `ADMIN_PANEL` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак доступа к административной панели',
  `ADMIN_PANEL_ADD_ACCOUNT` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак добавления пользователей',
  `ADMIN_PANEL_EDIT_ACCOUNT` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак редактирования пользователей',
  `ADMIN_PANEL_DELETE_ACCOUNT` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак аннулирования пользователей',
  `ADMIN_PANEL_SET_NUMERATOR` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак установки нумератора',
  `ADMIN_PANEL_JOURNAL` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак доступа к журналу событий',
  `IS_ANNULATED` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак аннулированного пользователя'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Дамп данных таблицы `ACCOUNT`
--

INSERT INTO `ACCOUNT` (`ID`, `CODE`, `LOGIN`, `PASSWORD`, `SURNAME`, `NAME`, `PATRONYMIC`, `ADRESS`, `PHONE`, `SEX`, `PURCHASE_INVOICE`, `SALES_INVOICE`, `REPORTS`, `REPORT_PURCHASE_INVOICE`, `REPORT_UN_ISSUE_PURCHASE_INVOICE`, `REPORT_SALES_INVOICE`, `REPORT_UN_ISSUE_SALES_INVOICE`, `REPORT_INCOME_EXPENSE`, `COUNTERPARTY`, `COUNTERPARTY_PROVIDER`, `COUNTERPARTY_PROVIDER_ADD`, `COUNTERPARTY_PROVIDER_EDIT`, `COUNTERPARTY_PROVIDER_DELETE`, `COUNTERPARTY_CUSTOMER`, `COUNTERPARTY_CUSTOMER_ADD`, `COUNTERPARTY_CUSTOMER_EDIT`, `COUNTERPARTY_CUSTOMER_DELETE`, `WAREHOUSE`, `WAREHOUSE_ADD_PRODUCT`, `WAREHOUSE_EDIT_PRODUCT`, `WAREHOUSE_ANNULATE_PRODUCT`, `WAREHOUSE_EDIT_COUNT`, `ADMIN_PANEL`, `ADMIN_PANEL_ADD_ACCOUNT`, `ADMIN_PANEL_EDIT_ACCOUNT`, `ADMIN_PANEL_DELETE_ACCOUNT`, `ADMIN_PANEL_SET_NUMERATOR`, `ADMIN_PANEL_JOURNAL`, `IS_ANNULATED`) VALUES
(1, '', 'admin', 'æ­¥³Íé°v¡¤ãç·äÆÕáÂî·ëÈ¿®áêÇ»¿~ª·Á§ôµ»æÊ¯¿¨¾', 'Ряполов', 'Александр', 'Николаевич', '', '', 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0),
(4, '', 'tkachenko', 'æ­¥³Íé°v¡¤ãç·äÆÕáÂî·ëÈ¿®áêÇ»¿~ª·Á§ôµ»æÊ¯¿¨¾', 'Ткаченко', 'Виктория', 'Викторовна', '', '', 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0),
(5, '', 'gil', 'Þ´«äº¬¢´Ô­Áò¿¥à´Î¶Î«~¾Ç§¹ÀÀ¬q¹¼¤­Ø¥ÀÑ°ÓÏâë', 'Гиль', 'Ольга', 'Наумовна', '', '', 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

-- --------------------------------------------------------

--
-- Структура таблицы `COUNTERPARTY`
--

CREATE TABLE `COUNTERPARTY` (
  `ID` int(11) NOT NULL COMMENT 'идентификатор',
  `CODE` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT 'код контрагента',
  `SURNAME` varchar(50) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT 'фамилия контрагента',
  `NAME` varchar(50) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT 'имя контрагента',
  `PATRONYMIC` varchar(50) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT 'отчество контрагента',
  `NOMINATION` varchar(500) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT 'наименование контрагента',
  `ADRESS` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT 'адрес контрагента (зашифрованный)',
  `PHONE` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT 'телефон контрагента (зашифрованный)',
  `IS_PROVIDER` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак поставщика',
  `IS_ANNULATED` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак аннулированного контрагента'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Дамп данных таблицы `COUNTERPARTY`
--

INSERT INTO `COUNTERPARTY` (`ID`, `CODE`, `SURNAME`, `NAME`, `PATRONYMIC`, `NOMINATION`, `ADRESS`, `PHONE`, `IS_PROVIDER`, `IS_ANNULATED`) VALUES
(7, '', '', '', '', 'Покупатель', '', '', 0, 0),
(8, '', '', '', '', 'ООО GreenLeaf', '', '', 1, 0),
(9, '', '', '', '', 'СЦ Москва', '', '', 1, 0),
(10, '', 'Мазаева', 'Татьяна', 'Аркадьевна', 'СЦ Самара', '', '', 1, 0);

-- --------------------------------------------------------

--
-- Структура таблицы `JOURNAL`
--

CREATE TABLE `JOURNAL` (
  `ID` int(11) NOT NULL COMMENT 'идентификатор',
  `DATE` datetime NOT NULL COMMENT 'дата события',
  `ID_ACCOUNT` int(11) NOT NULL COMMENT 'ID пользователя',
  `ACT` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL COMMENT 'описание события'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `NUMERATOR`
--

CREATE TABLE `NUMERATOR` (
  `ID` int(11) NOT NULL COMMENT 'идентификатор',
  `NOMINATION` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL COMMENT 'наименование нумератора',
  `VALUE` int(11) NOT NULL COMMENT 'значение нумератора'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Дамп данных таблицы `NUMERATOR`
--

INSERT INTO `NUMERATOR` (`ID`, `NOMINATION`, `VALUE`) VALUES
(1, 'Приходная накладная', 0),
(2, 'Расходная накладная', 0);

-- --------------------------------------------------------

--
-- Структура таблицы `PRODUCT`
--

CREATE TABLE `PRODUCT` (
  `ID` int(11) NOT NULL COMMENT 'идентификатор',
  `PRODUCT_CODE` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL COMMENT 'код товара',
  `NOMINATION` varchar(500) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL COMMENT 'наименование товара',
  `COUNT_IN_PACKAGE` double NOT NULL COMMENT 'количество в упаковке',
  `COST` double NOT NULL COMMENT 'стоимость реализации',
  `COUPON` double NOT NULL COMMENT 'купон по реализации',
  `COST_PURCHASE` double NOT NULL COMMENT 'закупочная стоимость',
  `COUPON_PURCHASE` double NOT NULL COMMENT 'закупочная стоимость купона',
  `COUNT` double NOT NULL DEFAULT '0' COMMENT 'количество на складе',
  `LOCKED_COUNT` double NOT NULL DEFAULT '0' COMMENT 'заблокированное количество',
  `ID_UNIT` int(11) NOT NULL COMMENT 'ID единицы измерения',
  `IS_ANNULATED` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак аннулированного товара'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Дамп данных таблицы `PRODUCT`
--

INSERT INTO `PRODUCT` (`ID`, `PRODUCT_CODE`, `NOMINATION`, `COUNT_IN_PACKAGE`, `COST`, `COUPON`, `COST_PURCHASE`, `COUPON_PURCHASE`, `COUNT`, `LOCKED_COUNT`, `ID_UNIT`, `IS_ANNULATED`) VALUES
(12, 'CEA031', '«Я люблю жизнь» салфетки', 10, 354, 102, 354, 102, 0, 0, 1, 0);

-- --------------------------------------------------------

--
-- Структура таблицы `PURCHASE_INVOICE`
--

CREATE TABLE `PURCHASE_INVOICE` (
  `ID` int(11) NOT NULL COMMENT 'идентификатор',
  `NUMBER` int(11) NOT NULL DEFAULT '0' COMMENT 'номер накладной',
  `ID_ACCOUNT` int(11) NOT NULL DEFAULT '0' COMMENT 'ID создателя накладной',
  `ID_COUNTERPARTY` int(11) NOT NULL DEFAULT '0' COMMENT 'ID контрагента',
  `DATE` date DEFAULT NULL COMMENT 'дата проведения накладной',
  `CREATE_DATE` date DEFAULT NULL COMMENT 'дата создания накладной',
  `COST` double NOT NULL DEFAULT '0' COMMENT 'итоговая стоимость по накладной',
  `COUPON` double NOT NULL DEFAULT '0' COMMENT 'итоговый купон по накладной',
  `IS_ISSUED` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак проведенной накладной',
  `IS_LOCKED` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак заблокированной накладной'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `PURCHASE_INVOICE_UNIT`
--

CREATE TABLE `PURCHASE_INVOICE_UNIT` (
  `ID` int(11) NOT NULL COMMENT 'идентификатор',
  `ID_INVOICE` int(11) NOT NULL COMMENT 'ID приходной накладной',
  `ID_PRODUCT` int(11) NOT NULL COMMENT 'ID товара',
  `COUNT` int(11) NOT NULL COMMENT 'количество товара',
  `PRODUCT_COST` double NOT NULL COMMENT 'стоимость товара',
  `PRODUCT_COUPON` double NOT NULL COMMENT 'купон товара',
  `COST` double NOT NULL COMMENT 'итоговая стоимость',
  `COUPON` double NOT NULL COMMENT 'итоговый купон'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `SALES_INVOICE`
--

CREATE TABLE `SALES_INVOICE` (
  `ID` int(11) NOT NULL COMMENT 'идентификатор',
  `NUMBER` int(11) NOT NULL DEFAULT '0' COMMENT 'номер накладной',
  `ID_ACCOUNT` int(11) NOT NULL DEFAULT '0' COMMENT 'ID создателя накладной',
  `ID_COUNTERPARTY` int(11) NOT NULL DEFAULT '0' COMMENT 'ID контрагента',
  `DATE` date DEFAULT NULL COMMENT 'дата проведения накладной',
  `CREATE_DATE` date DEFAULT NULL COMMENT 'дата создания накладной',
  `COST` double NOT NULL DEFAULT '0' COMMENT 'итоговая стоимость по накладной',
  `COUPON` double NOT NULL DEFAULT '0' COMMENT 'итоговый купон по накладной',
  `IS_ISSUED` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак проведенной накладной',
  `IS_LOCKED` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак заблокированной накладной'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `SALES_INVOICE_UNIT`
--

CREATE TABLE `SALES_INVOICE_UNIT` (
  `ID` int(11) NOT NULL COMMENT 'идентификатор',
  `ID_INVOICE` int(11) NOT NULL COMMENT 'ID расходной накладной',
  `ID_PRODUCT` int(11) NOT NULL COMMENT 'ID товара',
  `COUNT` int(11) NOT NULL COMMENT 'количество товара',
  `PRODUCT_COST` double NOT NULL COMMENT 'стоимость товара',
  `PRODUCT_COUPON` double NOT NULL COMMENT 'купон товара',
  `COST` double NOT NULL COMMENT 'итоговая стоимость',
  `COUPON` double NOT NULL COMMENT 'итоговый купон'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `SETTINGS`
--

CREATE TABLE `SETTINGS` (
  `ID` int(11) NOT NULL COMMENT 'идентификатор',
  `NOMINATION` varchar(100) COLLATE utf8_unicode_ci NOT NULL COMMENT 'наименование настройки',
  `VALUE` varchar(1000) COLLATE utf8_unicode_ci NOT NULL COMMENT 'значение настройки'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Дамп данных таблицы `SETTINGS`
--

INSERT INTO `SETTINGS` (`ID`, `NOMINATION`, `VALUE`) VALUES
(1, 'Наименование организации для накладной', 'СЦ Greenleaf Тольятти');

-- --------------------------------------------------------

--
-- Структура таблицы `UNIT`
--

CREATE TABLE `UNIT` (
  `ID` int(11) NOT NULL COMMENT 'идентификатор',
  `NOMINATION` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL COMMENT 'наименование единицы измерения',
  `IS_ANNULATED` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'признак аннулированной единицы измерения'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Дамп данных таблицы `UNIT`
--

INSERT INTO `UNIT` (`ID`, `NOMINATION`, `IS_ANNULATED`) VALUES
(1, 'шт', 0),
(2, 'кг', 0);

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `ACCOUNT`
--
ALTER TABLE `ACCOUNT`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `ID` (`ID`),
  ADD UNIQUE KEY `ID_2` (`ID`),
  ADD UNIQUE KEY `ID_3` (`ID`),
  ADD UNIQUE KEY `ID_4` (`ID`);

--
-- Индексы таблицы `COUNTERPARTY`
--
ALTER TABLE `COUNTERPARTY`
  ADD PRIMARY KEY (`ID`);

--
-- Индексы таблицы `JOURNAL`
--
ALTER TABLE `JOURNAL`
  ADD PRIMARY KEY (`ID`);

--
-- Индексы таблицы `NUMERATOR`
--
ALTER TABLE `NUMERATOR`
  ADD PRIMARY KEY (`ID`);

--
-- Индексы таблицы `PRODUCT`
--
ALTER TABLE `PRODUCT`
  ADD PRIMARY KEY (`ID`);

--
-- Индексы таблицы `PURCHASE_INVOICE`
--
ALTER TABLE `PURCHASE_INVOICE`
  ADD PRIMARY KEY (`ID`);

--
-- Индексы таблицы `PURCHASE_INVOICE_UNIT`
--
ALTER TABLE `PURCHASE_INVOICE_UNIT`
  ADD PRIMARY KEY (`ID`);

--
-- Индексы таблицы `SALES_INVOICE`
--
ALTER TABLE `SALES_INVOICE`
  ADD PRIMARY KEY (`ID`);

--
-- Индексы таблицы `SALES_INVOICE_UNIT`
--
ALTER TABLE `SALES_INVOICE_UNIT`
  ADD PRIMARY KEY (`ID`);

--
-- Индексы таблицы `SETTINGS`
--
ALTER TABLE `SETTINGS`
  ADD PRIMARY KEY (`ID`);

--
-- Индексы таблицы `UNIT`
--
ALTER TABLE `UNIT`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `NAME` (`NOMINATION`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `ACCOUNT`
--
ALTER TABLE `ACCOUNT`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT COMMENT 'идентификатор', AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT для таблицы `COUNTERPARTY`
--
ALTER TABLE `COUNTERPARTY`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT COMMENT 'идентификатор', AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT для таблицы `JOURNAL`
--
ALTER TABLE `JOURNAL`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT COMMENT 'идентификатор', AUTO_INCREMENT=77;

--
-- AUTO_INCREMENT для таблицы `NUMERATOR`
--
ALTER TABLE `NUMERATOR`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT COMMENT 'идентификатор', AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT для таблицы `PRODUCT`
--
ALTER TABLE `PRODUCT`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT COMMENT 'идентификатор', AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT для таблицы `PURCHASE_INVOICE`
--
ALTER TABLE `PURCHASE_INVOICE`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT COMMENT 'идентификатор', AUTO_INCREMENT=69;

--
-- AUTO_INCREMENT для таблицы `PURCHASE_INVOICE_UNIT`
--
ALTER TABLE `PURCHASE_INVOICE_UNIT`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT COMMENT 'идентификатор', AUTO_INCREMENT=60;

--
-- AUTO_INCREMENT для таблицы `SALES_INVOICE`
--
ALTER TABLE `SALES_INVOICE`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT COMMENT 'идентификатор', AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT для таблицы `SALES_INVOICE_UNIT`
--
ALTER TABLE `SALES_INVOICE_UNIT`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT COMMENT 'идентификатор', AUTO_INCREMENT=20;

--
-- AUTO_INCREMENT для таблицы `SETTINGS`
--
ALTER TABLE `SETTINGS`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT COMMENT 'идентификатор', AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT для таблицы `UNIT`
--
ALTER TABLE `UNIT`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT COMMENT 'идентификатор', AUTO_INCREMENT=3;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

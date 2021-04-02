-- phpMyAdmin SQL Dump
-- version 3.5.1
-- http://www.phpmyadmin.net
--
-- Хост: 127.0.0.1
-- Время создания: Апр 02 2021 г., 13:24
-- Версия сервера: 5.5.25
-- Версия PHP: 5.3.13

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- База данных: `agechat`
--

-- --------------------------------------------------------

--
-- Структура таблицы `globalmessages`
--

CREATE TABLE IF NOT EXISTS `globalmessages` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `senderId` int(11) NOT NULL,
  `messageText` varchar(255) NOT NULL,
  `dateTime` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `senderId` (`senderId`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=35 ;

--
-- Дамп данных таблицы `globalmessages`
--

INSERT INTO `globalmessages` (`id`, `senderId`, `messageText`, `dateTime`) VALUES
(2, 1, 'hello, kaban', '2021-03-26 19:13:19'),
(3, 3, 'Сам кабан!', '2021-03-26 19:14:42'),
(4, 2, 'сообщение юзерам', '2021-03-26 19:26:28'),
(5, 1, 'здарова', '2021-03-26 19:29:08'),
(6, 2, 'ку!', '2021-03-26 19:31:32'),
(7, 1, 'ku!', '2021-03-26 19:31:52'),
(8, 2, 'zdarova', '2021-03-26 19:33:23'),
(9, 3, 'zxc', '2021-03-26 19:34:46'),
(10, 3, 'hello', '2021-03-26 19:37:55'),
(11, 2, 'здарова', '2021-03-26 19:41:12'),
(12, 1, 'zxc', '2021-03-26 19:43:30'),
(13, 2, 'fsdgsf', '2021-03-26 19:44:49'),
(14, 2, 'gm', '2021-03-26 19:50:49'),
(16, 2, 'zzxc', '2021-03-26 20:00:30'),
(17, 2, 'test', '2021-03-26 23:06:11'),
(18, 1, 'test3', '2021-03-26 23:07:55'),
(19, 2, 'bank prikolov tut?', '2021-03-26 23:20:01'),
(20, 3, 'Всем привет!', '2021-03-26 23:43:30'),
(21, 1, 'kaban, tut?', '2021-03-26 23:48:45'),
(22, 3, 'kaban ne tut', '2021-03-26 23:51:29'),
(23, 1, 'Я тут', '2021-03-29 20:07:17'),
(24, 3, 'а я не тут', '2021-03-29 20:08:31'),
(25, 5, 'я аферист, здрасьте)', '2021-03-29 20:18:46'),
(26, 5, 'hello world', '2021-03-30 10:34:05'),
(27, 1, 'kaban ваще не здесь...', '2021-03-31 00:19:00'),
(28, 1, 'кабан ваще не здесь', '2021-03-31 00:19:32'),
(29, 5, 'кабан', '2021-03-31 00:20:10'),
(30, 1, 'Леонардо сефуентес есть?', '2021-03-31 00:42:08'),
(31, 2, 'нет(', '2021-03-31 00:42:49'),
(32, 2, 'hello', '2021-04-02 10:44:53'),
(33, 1, 'hi', '2021-04-02 10:45:10'),
(34, 1, 'Леонардо Сефуентес, ты тут?', '2021-04-02 10:47:01');

-- --------------------------------------------------------

--
-- Структура таблицы `messages`
--

CREATE TABLE IF NOT EXISTS `messages` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `senderId` int(11) NOT NULL,
  `receiverId` int(11) NOT NULL,
  `messageText` varchar(255) NOT NULL,
  `dateTime` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `senderId` (`senderId`,`receiverId`),
  KEY `receiverId` (`receiverId`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=23 ;

--
-- Дамп данных таблицы `messages`
--

INSERT INTO `messages` (`id`, `senderId`, `receiverId`, `messageText`, `dateTime`) VALUES
(1, 1, 2, 'ку, кабан', '2021-03-26 20:01:09'),
(2, 2, 1, 'privet', '2021-03-26 20:06:26'),
(3, 2, 1, '325423', '2021-03-26 20:09:39'),
(4, 3, 1, 'pm', '2021-03-26 20:11:34'),
(5, 2, 1, 'pm', '2021-03-26 20:14:47'),
(6, 2, 1, 'pmtest', '2021-03-26 23:08:20'),
(7, 3, 1, 'pm', '2021-03-26 23:23:14'),
(8, 3, 1, 'msg', '2021-03-26 23:24:09'),
(9, 2, 1, 'admin', '2021-03-26 23:32:02'),
(10, 3, 1, 'hi', '2021-03-26 23:33:35'),
(11, 3, 2, 'zdarova, kaban!', '2021-03-26 23:44:15'),
(12, 2, 3, 'nu zdravstvuy)', '2021-03-26 23:44:46'),
(13, 1, 3, 'ok, sps', '2021-03-26 23:52:06'),
(14, 2, 1, 'kaban na meste))', '2021-03-26 23:54:00'),
(15, 1, 3, 'как дела?', '2021-03-29 15:07:40'),
(17, 1, 2, 'как дела?', '2021-03-29 20:09:35'),
(18, 2, 1, 'хорошо', '2021-03-29 20:10:46'),
(19, 2, 5, 'zdarova', '2021-03-31 00:22:00'),
(20, 2, 1, 'admin', '2021-03-31 00:43:53'),
(21, 1, 3, 'zxc', '2021-03-31 00:46:49'),
(22, 2, 1, 'admin,привет', '2021-04-02 10:45:56');

-- --------------------------------------------------------

--
-- Структура таблицы `users`
--

CREATE TABLE IF NOT EXISTS `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `login` varchar(30) NOT NULL,
  `passwordHash` char(40) NOT NULL,
  `username` varchar(30) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `login` (`login`),
  UNIQUE KEY `username` (`username`),
  UNIQUE KEY `username_2` (`username`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=6 ;

--
-- Дамп данных таблицы `users`
--

INSERT INTO `users` (`id`, `login`, `passwordHash`, `username`) VALUES
(1, 'admin', '40bd001563085fc35165329ea1ff5c5ecbdbbeef', 'admin'),
(2, 'user1', '011c945f30ce2cbafc452f39840f025693339c42', 'kaban228'),
(3, 'user2', '011c945f30ce2cbafc452f39840f025693339c42', 'zdarova'),
(5, 'user3', '011c945f30ce2cbafc452f39840f025693339c42', 'aferist');

--
-- Ограничения внешнего ключа сохраненных таблиц
--

--
-- Ограничения внешнего ключа таблицы `globalmessages`
--
ALTER TABLE `globalmessages`
  ADD CONSTRAINT `globalmessages_ibfk_1` FOREIGN KEY (`senderId`) REFERENCES `users` (`id`) ON DELETE CASCADE;

--
-- Ограничения внешнего ключа таблицы `messages`
--
ALTER TABLE `messages`
  ADD CONSTRAINT `messages_ibfk_1` FOREIGN KEY (`senderId`) REFERENCES `users` (`id`),
  ADD CONSTRAINT `messages_ibfk_2` FOREIGN KEY (`receiverId`) REFERENCES `users` (`id`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

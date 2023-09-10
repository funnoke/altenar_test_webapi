# altenar_test_webapi
**Look branch. I started from NoMVC then I decided to change to MVC.**

# TASK:

Общие требования: 
Приложение должно быть написано на .NET Core. 
Выбор БД свободный (MS SQL, Postgresql, MongoDB и тд)

Задание не обязательно выполнять целиком. Можно сделать частично на сколько хватит опыта, знаний и времени.

Юнит тесты в коде приветствуются

Желательно придерживаться данного стиля кодирования
https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md

Задача:
Реализовать Web API для фронтенда букмекерской системы. 
Для тестирования запросов можно использовать postman


1 Спортивная часть

БД 

Добавить таблицу  спортивные события (ID события, Дата события, Название, Коэффициент на 1ю команду, Коэффициент на ничью, коэффициент на 2ю команду)

Заполнить данными
Например
(1, 13.06.2019 03:30 UTC, SANTOS FC VS. SC CORINTHIANS SP, 1.65, 1.6, 2.8)

API

Публичное API которое через get позволяет получать
- Все события на ближайшие сутки
- Все события за интервал дат 

2 Ставочная часть

БД

Добавить таблицу Игрок (ID игрока, Логин, Хеш пароля, Баланс)

Заполнить данными (добавить 2-3 игроков)

Добавить таблицу Ставки(ID ставки,ID игрока, Дата когда поставили ставку,ID события, Тип коэффициента, Коэффициент на момент ставки, Количество поставленных денег)

Тип коеффициента - 0 - 1я команда победит, 1 - ничья, 2 - вторая команда победит


API 

Для ставочного API необходима авторизация (Логин и Пароль игрока). 
Я бы рекомендовал делать jwt token авторизацию, но можно любую другую на свой выбор

В API Должны быть POST методы
-Поставить ставку
-Получить список всех своих ставок 

При постановке ставки нужно проверять баланс игрока достаточно ли у него денег, если недостаточно то возвращать соответствующую ошибку. Если достаточно то ставить ставку и уменьшать баланс на количество поставленных денег

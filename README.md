# XrmTask1
## Тестовое задание № 1 http://xrm.ru/job/test_task/
В качестве подопытного выбран каталог ИКЕА http://www.ikea.com/ru/ru/catalog/allproducts/

Структура каталога следующая:
* Отделы (представлены классом *Department*)
  * Категории (класс *SubCategory*)
    * Товары (класс *Product*)

Нюансов у данного каталога два:
  1. один и тот же товар может находиться в нескольких категориях
  2. в каталоге на исходной странице http://www.ikea.com/ru/ru/catalog/allproducts/ на одну категорию может быть несколько ссылок

При парсинге каталога для исключения дубликатов со сложностью О(1) используется *Dictionary*

Функциональность в приложении поделена на слой UI (WPF с паттерном MVVM) и слой доступа к данным (ORM - Entity Framework), для уменьшения связности слоев реализован примитивный репозиторий (класс *IkeaRepository*).

Реализованные в приложении возможности:
  1. Загрузка каталога с сайта в память
  2. Сохранение каталога в базу данных
  3. Загрузка сохраненного каталога из базы данных
  4. Отображение информации о товарах (изображение, название, описание, цена) с группировкой по отделам и категориям

Что не реализовано:
  1. Поиск в каталоге - без морфологии реализуется с помощью LINQ запросов
  2. Сортировка по названию/цене (цена вообще созраняется в string)
  3. Баг - если на товар есть скидка, парсер не находит цену

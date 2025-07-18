use Альбион_old
go

--Триггер, меняющий статус заказа на "Завершен" при ненулевой дате выполнения заказа
create trigger статус_заказа
on Заказ
after insert, update as
begin
	update Заказ
	set ID_Статуса = 7
	where ДатаВыполнения is not null;
end;
go

--Хранимая процедура, расчитывающая сумму заказа
create procedure расчет_суммы
    @orderID int
as
begin
    set nocount on;
    -- Проверяем, существует ли заказ
    if not exists (select 1 from Заказ where ID_Заказа = @orderID)
    begin
        raiserror('Заказ с указанным ID не найден.', 16, 1);
        return;
    end
    -- Объявляем переменную для хранения суммы
    declare @totalprice decimal(18,2);
    -- Вычисляем общую сумму заказа
    select @totalprice = sum(t.Цена * tz.Количество)
    from Товар_Заказ tz
    join Товары t on tz.ID_Товара = t.ID_Товара
    where tz.ID_Заказа = @orderID;
    -- Если товаров нет, устанавливаем сумму в 0
    set @totalprice = isnull(@totalprice, 0);
    -- Обновляем сумму в заказе
    update Заказ
    set Сумма = @totalprice
    where ID_Заказа = @orderID;
end;

exec расчет_суммы @orderID = 15;


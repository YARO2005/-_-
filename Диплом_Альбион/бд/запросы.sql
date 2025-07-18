use �������_old
go

--�������, �������� ������ ������ �� "��������" ��� ��������� ���� ���������� ������
create trigger ������_������
on �����
after insert, update as
begin
	update �����
	set ID_������� = 7
	where �������������� is not null;
end;
go

--�������� ���������, ������������� ����� ������
create procedure ������_�����
    @orderID int
as
begin
    set nocount on;
    -- ���������, ���������� �� �����
    if not exists (select 1 from ����� where ID_������ = @orderID)
    begin
        raiserror('����� � ��������� ID �� ������.', 16, 1);
        return;
    end
    -- ��������� ���������� ��� �������� �����
    declare @totalprice decimal(18,2);
    -- ��������� ����� ����� ������
    select @totalprice = sum(t.���� * tz.����������)
    from �����_����� tz
    join ������ t on tz.ID_������ = t.ID_������
    where tz.ID_������ = @orderID;
    -- ���� ������� ���, ������������� ����� � 0
    set @totalprice = isnull(@totalprice, 0);
    -- ��������� ����� � ������
    update �����
    set ����� = @totalprice
    where ID_������ = @orderID;
end;

exec ������_����� @orderID = 15;


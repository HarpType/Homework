namespace Tests

module EvenNumbers = 
    open NUnit.Framework
    open FsUnit
    open CountingEvenNumbers

    /// ������������ ���������� ������ ����� 3 ���� ������� ���������,
    /// ��������� ��������� � ������.
    let countEvenNumThreeTimes list =
        (countEvenNumWithMap list, countEvenNumWithFilter list,
            countEvenNumWithFold list)

    [<Test>]
    let ``[1..10] list contains 5 even numbers`` ()=
        [1..10] |> countEvenNumThreeTimes |> should equal (5, 5, 5)

    [<Test>]
    let ``Epmty list contains 0 even numbers`` ()=
        [] |> countEvenNumThreeTimes |> should equal (0, 0, 0)
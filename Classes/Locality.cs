namespace Yandex_Weather_API.Classes;

//класс для населенных пунктов
public class Locality
{
    //долгота и широта в формате Яндекс АПИ
    public string Coordinates { get; private set; }
    //наименование населенного пункта
    public string Name { get; private set; }

    public Locality(string coordinates, string name)
    {
        Coordinates = coordinates;
        Name = name;
    }
    
    //метод для выбора пользователем одного населенного пункта из нескольких
    public static Locality SelectLocality(List<Locality> localities)
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Выберите номер населенного пункта из доступных:");
            for (int i = 0; i < localities.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i+1, localities[i].Name);
            }

            string selection = Console.ReadLine();

            if (uint.TryParse(selection, out uint number) && number-1 < localities.Count)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Выбран населенный пункт {0}", localities[(int)number-1].Name);
                return localities[(int)number-1];
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Выбран неверный номер населенного пункта");
            }
        }
    }
}
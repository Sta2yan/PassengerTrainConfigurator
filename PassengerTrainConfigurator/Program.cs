using System;
using System.Collections.Generic;

namespace PassengerTrainConfigurator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();

            CommandCentre commandCenter = new CommandCentre();
            Train train = new Train();
            int maximumPassengers = 100;
            int minimumPassengers = 30;
            int maximumTicketPrice = 2000;
            int minimumTicketPrice = 1000;
            int numberTrainCar = 10;
            int maximumTrainCarPlace = 16;
            int minimumTrainCarPlace = 10;
            int ticketPrice = random.Next(minimumTicketPrice, maximumTicketPrice);

            bool isOpen = true;

            while (isOpen)
            {
                int passengers = random.Next(minimumPassengers, maximumPassengers);
                Console.WriteLine($"Текущий рейс: ");
                train.ShowInfo();
                Console.WriteLine($"\n{(int)ControlMenuCommand.CreateDirection}. {ControlMenuCommand.CreateDirection}" +
                                  $"\n{(int)ControlMenuCommand.SellTickets}. {ControlMenuCommand.SellTickets}" +
                                  $"\n{(int)ControlMenuCommand.FormTrain}. {ControlMenuCommand.FormTrain}" +
                                  $"\n{(int)ControlMenuCommand.SendTrain}. {ControlMenuCommand.SendTrain}" +
                                  $"\n{(int)ControlMenuCommand.Exit}. {ControlMenuCommand.Exit}");
                int userInput = GetNumber(Console.ReadLine());
                Console.Clear();

                switch (userInput)
                {
                    case (int)ControlMenuCommand.CreateDirection:
                        train.CreateDirection();
                        break;
                    case (int)ControlMenuCommand.SellTickets:
                        commandCenter.SellTicket(train, ticketPrice, passengers);
                        break;
                    case (int)ControlMenuCommand.FormTrain:
                        train.FormTrain(GetTrainCars(numberTrainCar, minimumTrainCarPlace, maximumTrainCarPlace));
                        break;
                    case (int)ControlMenuCommand.SendTrain:
                        commandCenter.SendTrain(ref train);
                        break;
                    case (int)ControlMenuCommand.Exit:
                        isOpen = false;
                        break;
                }

                Console.ReadKey();
                Console.Clear();
            }

            Console.WriteLine("Выход ...");
        }

        public static int GetNumber(string numberText)
        {
            int number;

            while (int.TryParse(numberText, out number) == false)
            {
                Console.WriteLine("Повторите попытку:");
                numberText = Console.ReadLine();
            }

            return number;
        }

        public static List<TrainCar> GetTrainCars(int number, int minimumPlace, int maximumPlace)
        {
            Random random = new Random();
            List<TrainCar> trainCars = new List<TrainCar>();

            for (int i = 0; i < number; i++)
            {
                trainCars.Add(new TrainCar(random.Next(minimumPlace, maximumPlace)));
            }

            return trainCars;
        }
    }

    enum ControlMenuCommand
    {
        CreateDirection = 1,
        SellTickets,
        FormTrain,
        SendTrain,
        Exit
    }

    class CommandCentre
    {
        public void SellTicket(Train train, int ticketPrice, int number)
        {
            if (train.From == null || train.To == null)
            {
                Console.WriteLine("Не указан рейс!");
            }
            else
            {
                if (train.BoughtTicket <= 0)
                {
                    for (int i = 0; i < number; i++)
                    {
                        train.AddBoughtPlace(new Ticket(1000));
                    }

                    Console.WriteLine($"Продано {number} билетов");
                }
                else
                {
                    Console.WriteLine("Билеты распроданы");
                }
            }
        }

        public void SendTrain(ref Train train)
        {
            if (train.GetFreePlaces() > train.BoughtTicket)
            {
                Console.WriteLine($"{train.Name} отправляется ..." +
                                  $"\nИнформация о поезде:");
                train.ShowInfo();
                train = new Train();
            }
            else
            {
                Console.WriteLine("Ошибка");
            }
        }
    }

    class Train
    {
        private List<TrainCar> _trainCars = new();
        private List<Ticket> _soldTickets = new();

        public string From { get; private set; }
        public string To { get; private set; }
        public string Name { get; private set; }
        public int BoughtTicket { get { return _soldTickets.Count; }  }

        public Train(string name = null)
        {
            Random random = new Random();

            if (name == null)
                Name = $"Поезд_{random.Next(0, 100_000)}";
        }

        public void CreateDirection()
        {
            if (From == null || To == null)
            {
                Console.WriteLine("Укажите место отправки:");
                From = Console.ReadLine();
                Console.WriteLine("Укажите место прибытия:");
                To = Console.ReadLine();

                Console.WriteLine("Маршрут");
                ShowInfo();
                Console.WriteLine("Успешно задан");
            }
            else
            {
                Console.WriteLine("Маршрут уже задан");
            }
        }

        public void AddCar(TrainCar car)
        {
            _trainCars.Add(car);
        }

        public void AddBoughtPlace(Ticket ticket)
        {
            _soldTickets.Add(ticket);
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Название поезда - {Name} | Место отправки - {From} | Место прибытия - {To}" +
                              $" | Кол-во мест в поезде - {GetFreePlaces()} | Купленных билетов - {_soldTickets.Count}");
        }

        public void FormTrain(List<TrainCar> trainCars)
        {
            while (_soldTickets.Count > GetFreePlaces())
            {
                Console.WriteLine($"Кол-во оплаченных билетов: {_soldTickets.Count}");
                Console.WriteLine($"Кол-во свободных мест: {GetFreePlaces()}");
                Console.WriteLine();

                for (int i = 0; i < trainCars.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. Вагон | Мест: {trainCars[i].NumberPlaces}");
                }

                Console.WriteLine("\nВыберите вагон для поезда: ");
                int index = Program.GetNumber(Console.ReadLine()) - 1;

                if (index < 0 || index > trainCars.Count - 1)
                {
                    Console.WriteLine("Вагон не найден");
                }
                else
                {
                    _trainCars.Add(trainCars[index]);
                    trainCars.RemoveAt(index);
                    Console.WriteLine($"Вагон добавлен");
                }

                Console.ReadKey();
                Console.Clear();
            }

            Console.WriteLine("Нужные вагоны добавлены");
        }

        public int GetFreePlaces()
        {
            int freePlace = 0;

            for (int i = 0; i < _trainCars.Count; i++)
            {
                freePlace += _trainCars[i].NumberPlaces;
            }

            return freePlace;
        }
    }

    class TrainCar
    {
        public int NumberPlaces { get; private set; }

        public TrainCar(int places)
        {
            NumberPlaces = places;
        }
    }

    class Ticket
    {
        private static int _ids;
        public int Price { get; private set; }
        public int Id { get; private set; }

        public Ticket(int price)
        {
            Id = ++_ids;
            Price = price;
        }
    }
}
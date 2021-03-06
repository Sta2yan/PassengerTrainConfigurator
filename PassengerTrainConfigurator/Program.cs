using System;
using System.Collections.Generic;

namespace PassengerTrainConfigurator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Manager manager = new Manager();
            manager.Start();
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
    }
    
    enum ControlMenuCommand
    {
        CreateDirection = 1,
        SellTickets,
        FormTrain,
        SendTrain,
        Exit
    }

    class Manager
    {
        private Random _random;
        private Train _train;
        private Direction _direction;
        
        public bool IsOpen { get; private set; }

        public Manager()
        {
            _random = new Random();
            _train = new Train();
            _direction = new Direction();
            IsOpen = true;
        }

        public void Start()
        {
            while (IsOpen)
            {
                Console.WriteLine($"Текущий рейс: ");
                _train.ShowInfo();
                Console.WriteLine($"\n{(int)ControlMenuCommand.CreateDirection}. {ControlMenuCommand.CreateDirection}" +
                                  $"\n{(int)ControlMenuCommand.SellTickets}. {ControlMenuCommand.SellTickets}" +
                                  $"\n{(int)ControlMenuCommand.FormTrain}. {ControlMenuCommand.FormTrain}" +
                                  $"\n{(int)ControlMenuCommand.SendTrain}. {ControlMenuCommand.SendTrain}" +
                                  $"\n{(int)ControlMenuCommand.Exit}. {ControlMenuCommand.Exit}");
                int userInput = Program.GetNumber(Console.ReadLine());
                Console.Clear();

                switch (userInput)
                {
                    case (int)ControlMenuCommand.CreateDirection:
                        _direction.Create(_train);
                        break;
                    case (int)ControlMenuCommand.SellTickets:
                        SellTicket();
                        break;
                    case (int)ControlMenuCommand.FormTrain:
                        _train.Form(GetTrainCars());
                        break;
                    case (int)ControlMenuCommand.SendTrain:
                        _train.Send();
                        break;
                    case (int)ControlMenuCommand.Exit:
                        IsOpen = false;
                        break;
                }

                Console.ReadKey();
                Console.Clear();
            }

            Console.WriteLine("Выход ...");
        }

        public List<TrainCar> GetTrainCars()
        {
            int number = 10;
            int maximumPlace = 16;
            int minimumPlace = 10;
            List<TrainCar> trainCars = new List<TrainCar>();

            for (int i = 0; i < number; i++)
            {
                trainCars.Add(new TrainCar(_random.Next(minimumPlace, maximumPlace)));
            }

            return trainCars;
        }

        public void SellTicket()
        {
            int maximumPassengers = 100;
            int minimumPassengers = 30;
            int maximumTicketPrice = 2000;
            int minimumTicketPrice = 1000;
            int number = _random.Next(minimumPassengers, maximumPassengers);

            if (_train.Direction == null)
            {
                Console.WriteLine("Не указан рейс!");
            }
            else
            {
                if (_train.BoughtTicket <= 0)
                {
                    for (int i = 0; i < number; i++)
                    {
                        _train.AddBoughtPlace(new Ticket(_random.Next(minimumTicketPrice, maximumTicketPrice)));
                    }

                    Console.WriteLine($"Продано {number} билетов");
                }
                else
                {
                    Console.WriteLine("Билеты распроданы");
                }
            }
        }
    }

    class Direction
    {
        public string From { get; private set; }
        public string To { get; private set; }

        public void Create(Train train)
        {
            if (train.Direction == null)
            {
                Console.WriteLine("Укажите место отправки:");
                From = Console.ReadLine();
                Console.WriteLine("Укажите место прибытия:");
                To = Console.ReadLine();

                train.SetDiretion(this);

                Console.WriteLine("Маршрут");
                train.ShowInfo();
                Console.WriteLine("Успешно задан");
            }
            else
            {
                Console.WriteLine("Маршрут уже задан");
            }
        }
    }

    class Train
    {
        private List<TrainCar> _trainCars = new();
        private List<Ticket> _soldTickets = new();

        public Direction Direction { get; set; }
        public string Name { get; private set; }
        public int BoughtTicket { get { return _soldTickets.Count; } private set { } }

        public Train(string name = null)
        {
            Random random = new Random();

            if (name == null)
                Name = $"Поезд_{random.Next(0, 100_000)}";
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
            Console.WriteLine($"Название поезда - {Name}" +
                              $"\nКол-во мест в поезде - {GetFreePlaces()} | Купленных билетов - {_soldTickets.Count}");
        }

        public void Form(List<TrainCar> trainCars)
        {
            while (_soldTickets.Count > GetFreePlaces() - 1)
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

        public void SetDiretion(Direction direction)
        {
            Direction = direction;
        }

        public void Send()
        {
            if (GetFreePlaces() > BoughtTicket)
            {
                Console.WriteLine($"{Name} отправляется ..." +
                                  $"\nИнформация о поезде:");
                ShowInfo();
                Reset();
            }
            else
            {
                Console.WriteLine("Ошибка");
            }
        }

        private int GetFreePlaces()
        {
            int freePlace = 0;

            for (int i = 0; i < _trainCars.Count; i++)
            {
                freePlace += _trainCars[i].NumberPlaces;
            }

            return freePlace;
        }

        private void Reset()
        {
            Random random = new Random();
            _trainCars = new();
            _soldTickets = new();
            Direction = null;
            Name = $"Поезд_{random.Next(0, 100_000)}";
            BoughtTicket = 0;
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
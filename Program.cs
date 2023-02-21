using System;
using System.Collections.Generic;

namespace ComputerClub
{
    class Program
    {
        static void Main(string[] args)
        {
            ComputerClub computerClub = new ComputerClub(8);
            computerClub.Work();
        }
    }

    class ComputerClub
    {
        private int _money = 0;
        private List<Computer> _computers = new List<Computer>();
        private Queue<Client> _clients = new Queue<Client>();
       
        public ComputerClub(int computerCount)
        {
            Random rand = new Random();

            for (int i = 0; i < computerCount; i++)
            {
                _computers.Add(new Computer(rand.Next(5, 15)));
            }

            CreateNewClients(25, rand);
        }

       public void CreateNewClients(int count, Random rand)
        {
            for(int i = 0; i < count; i++)
            {
                _clients.Enqueue(new Client(rand.Next(150, 250), rand));
            }
        }

        public void Work()
        {

            while (_clients.Count > 0) 
            {
                Client NewClient = _clients.Dequeue();
                Console.WriteLine($"Баланс компьютерного клуба: {_money}. Ждем нового клиента.");
                Console.WriteLine($"У нас новый клиент, и он хочет арендовать компьютер на {NewClient.DesiredMinutes} минут.\n");
                ShowAllComputer();

                Console.Write("\nВы предлагаете ему компьютер под номером:");
                string UserInput = Console.ReadLine();

                if(Int32.TryParse(UserInput, out int computerNumber))
                {
                    computerNumber--;
                    if(computerNumber >= 0 && computerNumber < _computers.Count)
                    {
                        if (_computers[computerNumber].IsTaken)
                        {
                            Console.WriteLine("Вы пытаетесь посадить клиента за компьютер, который уже занят!");

                        }
                        else
                        {
                            if (NewClient.CheckSolvency(_computers[computerNumber]))
                            {
                                Console.WriteLine("Клиент оплатил время и сел за компьютер " + (computerNumber + 1));
                                _money += NewClient.Pay();
                                _computers[computerNumber].BecomeTaken(NewClient);
                            }
                            else
                            {
                                Console.WriteLine("Клиент не смог оплатить.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Вы не знаете за какой стол посадить клиента! Клиент вам недоволен и ушел...");
                    }
                }
                else
                {
                    CreateNewClients(1, new Random());
                    Console.WriteLine("Неверный ввод! Повторите снова!");
                }
                Console.WriteLine("Чтобы перейти к другому клиенту, нажмите любую клавишу.");
                Console.ReadKey();
                Console.Clear();
                SpendOneMinute();
            }

        }

        private void ShowAllComputer()
        {
            Console.WriteLine("Список всех компьтеров:");
            for(int i = 0; i < _computers.Count; i++)
            {
                Console.Write(i + 1 + "-");
                _computers[i].ShowInfo();
            }
        }

        private void SpendOneMinute()
        {
            foreach (var computer in _computers)
                computer.SpendOneMinute();
        }

    }

    class Computer
    {
        private Client _client;
        private int _minutesRemaining;

        public bool IsTaken
        {
            get
            {
                return _minutesRemaining > 0;
            }
        }

        public int PricePerMinutes { get; private set; }

        public Computer(int pricePerMinutes)
        {
            PricePerMinutes = pricePerMinutes;
        }

        public void BecomeTaken(Client client)
        {
            _client = client;
            _minutesRemaining = _client.DesiredMinutes;
        }

        public void BecomeEmpty(Client client)
        {
            client = null;
        }

        public void SpendOneMinute()
        {
            _minutesRemaining--;
        }

        public void ShowInfo()
        {
            if (IsTaken)
                Console.WriteLine($"Компьютер занят! Осталось минут: {_minutesRemaining}");
            else
                Console.WriteLine($"Компьютер свободен! Цена за минуту: {PricePerMinutes}");
        }
    }

    class Client
    {
        private int _money;
        private int _moneyToPay;
        public int DesiredMinutes { get; private set; }

        public Client(int money, Random rand)
        {
            _money = money;
            DesiredMinutes = rand.Next(10, 31);
        }

        public bool CheckSolvency(Computer computer)
        {
            _moneyToPay = DesiredMinutes * computer.PricePerMinutes;
            if (_money >= _moneyToPay) return true;
            _moneyToPay = 0;
            return false;
            
        }

        public int Pay()
        {
            _money -= _moneyToPay;
            return _moneyToPay;
        }
    }
}

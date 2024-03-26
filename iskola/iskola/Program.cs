using System;
using System.Data.SQLite;
using System.Transactions;

namespace ConsoleApp3
{
    internal class Program
    {
        static List<Iskola> iskolak = new List<Iskola>();
        public static string connectionString = "Data Source=adatbazis.db;Version=3;";

        public static void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Iskola (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    Address TEXT
                );";

                using (var createTableCommand = new SQLiteCommand(createTableQuery, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                }

                string createTableQuery1 = @"
                CREATE TABLE IF NOT EXISTS Diakok (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                Age INT
                )";

                using (var createTableCommand1 = new SQLiteCommand(createTableQuery1, connection))
                {
                    createTableCommand1.ExecuteNonQuery();
                }

                string createTableQuery2 = @"
                CREATE TABLE IF NOT EXISTS Iskolaadat (
                IskolaId INT,
                DiakId INT
                )";
                
                using (var createTableCommand2 = new SQLiteCommand(createTableQuery2, connection))
                {
                    createTableCommand2.ExecuteNonQuery();
                }
            }
           
        }
            static void Main(string[] args)
            {
                InitializeDatabase();
                bool kilepes = false;

                while (!kilepes)
                {
                    
                    Console.WriteLine("Válassz egy műveletet: ");
                    Console.WriteLine("1 - Új iskola hozzáadása");
                    Console.WriteLine("2 - Új diák hozzáadása az iskolához");
                    Console.WriteLine("3 - Iskola adatainak megjelenítése");
                    Console.WriteLine("4 - Kilépés");

                    string valasztas = Console.ReadLine();

                    switch (valasztas)
                    {
                        case "1":
                            Console.Clear();
                            Console.WriteLine("Adja meg az iskola nevét: ");
                            string iskolaNev = Console.ReadLine();
                            Console.WriteLine("Adja meg az iskola címét: ");
                            string iskolaCim = Console.ReadLine();
                            AddIskola(new Iskola(iskolaNev, iskolaCim));
                            break;
                        case "2":
                            Console.WriteLine("Melyik iskolához szeretne diákot hozzáadni? (index)");
                            List<Iskola> iskolak1 = GetIskola();
                            int index1 = 1;
                                foreach (Iskola iskola in iskolak1)
                                {
                                    Console.WriteLine($"{index1} - {iskola.Nev}, {iskola.Cim}");
                                    index1++;
                                }
                            int iskolaIndex = Convert.ToInt32(Console.ReadLine());
                                Console.WriteLine("Adja meg a diák nevét: ");
                                string diakNev = Console.ReadLine();
                                Console.WriteLine("Adja meg a diák életkorát: ");
                                int diakKor = Convert.ToInt32(Console.ReadLine());
                                UjDiak(new Diak(diakNev, diakKor, iskolaIndex));


                            break;
                        case "3":
                            Console.WriteLine("Válassza ki az iskolát: ");
                            List<Iskola> iskolak = GetIskola();
                            int index = 1;
                            foreach (Iskola iskola in iskolak)
                            {
                                Console.WriteLine($"{index} - {iskola.Nev}, {iskola.Cim}");
                                index++; 
                            }
                            int kivlasztottIskolaIndex = Convert.ToInt32(Console.ReadLine());
                            GetDiak(kivlasztottIskolaIndex);
                            List<Diak> diakok = GetDiak(kivlasztottIskolaIndex);
                      
                            foreach (Diak diak in diakok)
                            {
                            Console.WriteLine($"Név: {diak.Nev}, Kor: {diak.Kor}");
                            }
                        break;
                        case "4":
                            kilepes = true;
                            break;
                        default:
                            Console.WriteLine("Érvénytelen választás! Kérem válasszon újra.");
                            break;
                    }
                }
            }

        // CREATEiskola
        public static void AddIskola(Iskola data)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO Iskola (Name, Address) VALUES (@Name, @Address)";

                using (var insertCommand = new SQLiteCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@Name", data.Nev);
                    insertCommand.Parameters.AddWithValue("@Address", data.Cim);
                    insertCommand.ExecuteNonQuery();
                }

            }
        }

        // Diak hozzaadasa
        public static void UjDiak(Diak data)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO Diakok (Name, Age) VALUES (@Name, @Age)";

                using (var insertCommand = new SQLiteCommand(insertQuery, connection))
                {
                insertCommand.Parameters.AddWithValue("@Name", data.Nev);
                insertCommand.Parameters.AddWithValue("@Age", data.Kor);
                insertCommand.ExecuteNonQuery();
                }

                // Lekérdezzük a beszúrt diák azonosítóját
                string selectQuery = "SELECT last_insert_rowid()";
                using (var selectCommand = new SQLiteCommand(selectQuery, connection))
                {
                    int diakId = Convert.ToInt32(selectCommand.ExecuteScalar());

      
                    string insertQuery1 = "INSERT INTO Iskolaadat (IskolaId, DiakId) VALUES (@IskolaId,@DiakId)";
                    using (var insertCommand1 = new SQLiteCommand(insertQuery1, connection))
                    {
                        insertCommand1.Parameters.AddWithValue("@IskolaId", data.Id);
                        insertCommand1.Parameters.AddWithValue("@DiakId", diakId);
                        insertCommand1.ExecuteNonQuery();
                    }
                }
    
            }
        }

        // Diak kiolvasasa
        public static List<Diak> GetDiak(int iskolaazonosito)
        {
            List<Diak> data = new List<Diak>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT diakok.Id, diakok.Name, diakok.Age FROM diakok " +
                                     "JOIN iskolaadat ON diakok.Id = iskolaadat.DiakId " +
                                     "JOIN iskola ON iskolaadat.IskolaId = iskola.Id " +
                                     "WHERE iskola.Id = @iskolaAzonosito";

                using (var selectCommand = new SQLiteCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@iskolaAzonosito", iskolaazonosito);

                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nev = reader["Name"].ToString();
                            int kor = Convert.ToInt32(reader["Age"]);
                            int id = Convert.ToInt32(reader["Id"]);

                            data.Add(new Diak(nev, kor, id));
                        }
                    }
                }

                return data;
            }
        }


        // Iskolak kiolvasasa
        public static List<Iskola> GetIskola()
        {
            List<Iskola> data = new List<Iskola>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT Iskola.Id, Iskola.Name, Iskola.Address FROM Iskola";
                          

                using (var selectCommand = new SQLiteCommand(selectQuery, connection))
                {

                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nev = reader["Name"].ToString();
                            string cim = reader["Address"].ToString();
                            int id = Convert.ToInt32(reader["Id"]);

                            data.Add(new Iskola(nev, cim));
                        }
                    }
                }

                return data;
            }
        }



    }
}

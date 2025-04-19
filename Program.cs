using System;
using System.Collections.Generic;
using System.Threading;

namespace MedievalTextAdventure
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Run();
        }
    }

    class Game
    {
        private Player player;
        private World world;
        private Difficulty difficulty;
        private Random random = new Random();
        private bool gameRunning = true;

        public void Run()
        {
            ShowIntro();
            InitializeGame();
            GameLoop();
            ShowOutro();
        }

        private void ShowIntro()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("╔══════════════════════════════════════════════════╗");
            Console.WriteLine("║            THE CURSED CROWN OF ALDRICH          ║");
            Console.WriteLine("╚══════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine("\nA text adventure of mystery, danger, and dark magic");
            Console.WriteLine("\nPress any key to begin...");
            Console.ReadKey(true);
        }

        private void InitializeGame()
        {
            SelectDifficulty();
            CreatePlayer();
            CreateWorld();
        }

        private void SelectDifficulty()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Select Difficulty:");
            Console.WriteLine("1. Novice (Easy)");
            Console.WriteLine("2. Knight (Normal)");
            Console.WriteLine("3. Legend (Hard)");
            Console.WriteLine("4. Masochist (Impossible)");
            Console.ResetColor();

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.KeyChar)
                {
                    case '1':
                        difficulty = Difficulty.Novice;
                        return;
                    case '2':
                        difficulty = Difficulty.Knight;
                        return;
                    case '3':
                        difficulty = Difficulty.Legend;
                        return;
                    case '4':
                        difficulty = Difficulty.Masochist;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nYou brave fool...");
                        Thread.Sleep(1500);
                        Console.ResetColor();
                        return;
                }
            }
        }

        private void CreatePlayer()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter your name, adventurer:");
            Console.ResetColor();
            string name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Sir Noname";
            }

            player = new Player(name, difficulty);
        }

        private void CreateWorld()
        {
            world = new World(difficulty);
            player.CurrentLocation = world.Locations[0]; // Starting location
        }

        private void GameLoop()
        {
            while (gameRunning)
            {
                Console.Clear();
                DisplayLocationInfo();
                DisplayPlayerStatus();
                ProcessInput();

                // Check for game over conditions
                if (player.Health <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nYou have met your demise...");
                    gameRunning = false;
                    Thread.Sleep(3000);
                }
                else if (player.HasCrown && player.CurrentLocation == world.Locations[0])
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nYou have returned with the Cursed Crown! The kingdom is saved!");
                    gameRunning = false;
                    Thread.Sleep(3000);
                }
            }
        }

        private void DisplayLocationInfo()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n== {player.CurrentLocation.Name} ==");
            Console.ResetColor();
            Console.WriteLine(player.CurrentLocation.Description);

            if (player.CurrentLocation.Enemy != null && !player.CurrentLocation.Enemy.IsDefeated)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nA {player.CurrentLocation.Enemy.Name} stands before you!");
                Console.ResetColor();
            }

            if (player.CurrentLocation.Item != null && !player.CurrentLocation.Item.IsTaken)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"\nYou see a {player.CurrentLocation.Item.Name} here.");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nExits:");
            foreach (var exit in player.CurrentLocation.Exits)
            {
                Console.WriteLine($"{exit.Key.ToUpper()}: {exit.Value.Name}");
            }
            Console.ResetColor();
        }

        private void DisplayPlayerStatus()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n{player.Name}");

            // Health display with color coding
            if (player.Health > 70)
                Console.ForegroundColor = ConsoleColor.Green;
            else if (player.Health > 30)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine($"Health: {player.Health}/100");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Gold: {player.Gold}");
            Console.WriteLine($"Level: {player.Level}");
            Console.WriteLine($"XP: {player.XP}/{player.XPToNextLevel}");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nInventory:");
            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("Empty");
            }
            else
            {
                foreach (var item in player.Inventory)
                {
                    Console.WriteLine($"- {item.Name}");
                }
            }

            if (player.HasCrown)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("\nYou carry the Cursed Crown!");
            }
            Console.ResetColor();
        }

        private void ProcessInput()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\nWhat will you do? (Move: N/E/S/W, Take, Use, Fight, Examine, Quit)");
            Console.ResetColor();

            string input = Console.ReadLine().ToLower();

            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            string[] commands = input.Split(' ');
            string action = commands[0];

            switch (action)
            {
                case "n":
                case "e":
                case "s":
                case "w":
                    MovePlayer(action[0]);
                    break;
                case "take":
                    TakeItem();
                    break;
                case "use":
                    if (commands.Length > 1)
                    {
                        UseItem(string.Join(" ", commands, 1, commands.Length - 1));
                    }
                    else
                    {
                        Console.WriteLine("Use what?");
                    }
                    break;
                case "fight":
                    StartCombat();
                    break;
                case "examine":
                    Examine();
                    break;
                case "quit":
                    gameRunning = false;
                    Console.WriteLine("Farewell, adventurer...");
                    Thread.Sleep(1000);
                    break;
                default:
                    Console.WriteLine("I don't understand that command.");
                    break;
            }
        }

        private void MovePlayer(char direction)
        {
            if (player.CurrentLocation.Exits.ContainsKey(direction.ToString()))
            {
                Location newLocation = player.CurrentLocation.Exits[direction.ToString()];

                // Check if location requires a key
                if (newLocation.RequiredKey != null)
                {
                    if (player.HasItem(newLocation.RequiredKey))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"You use the {newLocation.RequiredKey.Name} to unlock the path.");
                        Console.ResetColor();
                        player.CurrentLocation = newLocation;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"The path is locked! You need a {newLocation.RequiredKey.Name}.");
                        Console.ResetColor();
                        return;
                    }
                }
                else
                {
                    player.CurrentLocation = newLocation;
                }

                // Random chance of ambush when moving
                if (random.Next(1, 101) > 70) // 30% chance
                {
                    RandomEncounter();
                }
            }
            else
            {
                Console.WriteLine("You can't go that way.");
            }
        }

        private void RandomEncounter()
        {
            if (player.CurrentLocation.Enemy == null || player.CurrentLocation.Enemy.IsDefeated)
            {
                Enemy randomEnemy = Enemy.GenerateRandomEnemy(difficulty, player.Level);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nA wild {randomEnemy.Name} ambushes you!");
                Console.ResetColor();

                Combat combat = new Combat(player, randomEnemy);
                combat.Start();
            }
        }

        private void TakeItem()
        {
            if (player.CurrentLocation.Item != null && !player.CurrentLocation.Item.IsTaken)
            {
                if (player.Inventory.Count < 10) // Inventory limit
                {
                    player.AddItem(player.CurrentLocation.Item);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"You take the {player.CurrentLocation.Item.Name}.");
                    Console.ResetColor();
                    player.CurrentLocation.Item.IsTaken = true;
                    player.CurrentLocation.Item = null;
                }
                else
                {
                    Console.WriteLine("Your inventory is full!");
                }
            }
            else
            {
                Console.WriteLine("There's nothing here to take.");
            }
        }

        private void UseItem(string itemName)
        {
            Item item = player.Inventory.Find(i => i.Name.ToLower() == itemName.ToLower());

            if (item != null)
            {
                switch (item.Type)
                {
                    case ItemType.HealthPotion:
                        player.Health = Math.Min(100, player.Health + 30);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"You drink the {item.Name} and restore 30 health!");
                        Console.ResetColor();
                        player.RemoveItem(item);
                        break;
                    case ItemType.Key:
                        Console.WriteLine("This key can be used to unlock certain paths.");
                        break;
                    case ItemType.Weapon:
                        player.EquippedWeapon = (Weapon)item;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"You equip the {item.Name}!");
                        Console.ResetColor();
                        break;
                    case ItemType.Armor:
                        player.EquippedArmor = (Armor)item;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"You equip the {item.Name}!");
                        Console.ResetColor();
                        break;
                    case ItemType.Crown:
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine("You hold the Cursed Crown... you feel its dark power.");
                        Console.ResetColor();
                        break;
                    default:
                        Console.WriteLine($"You use the {item.Name}, but nothing happens.");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"You don't have a {itemName}.");
            }
        }

        private void StartCombat()
        {
            if (player.CurrentLocation.Enemy != null && !player.CurrentLocation.Enemy.IsDefeated)
            {
                Combat combat = new Combat(player, player.CurrentLocation.Enemy);
                combat.Start();

                if (player.CurrentLocation.Enemy.IsDefeated)
                {
                    // Enemy might drop loot
                    if (random.Next(1, 101) > 60) // 40% chance
                    {
                        Item droppedItem = Item.GenerateRandomItem(difficulty, player.Level);
                        player.CurrentLocation.Item = droppedItem;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"\nThe {player.CurrentLocation.Enemy.Name} dropped a {droppedItem.Name}!");
                        Console.ResetColor();
                    }

                    player.CurrentLocation.Enemy = null;
                }
            }
            else
            {
                Console.WriteLine("There's nothing here to fight.");
            }
        }

        private void Examine()
        {
            if (player.CurrentLocation.Enemy != null && !player.CurrentLocation.Enemy.IsDefeated)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"\n{player.CurrentLocation.Enemy.Name}");
                Console.WriteLine($"Health: {player.CurrentLocation.Enemy.Health}");
                Console.WriteLine($"Damage: {player.CurrentLocation.Enemy.Damage}");
                Console.WriteLine($"Defense: {player.CurrentLocation.Enemy.Defense}");
                Console.ResetColor();
            }
            else if (player.CurrentLocation.Item != null && !player.CurrentLocation.Item.IsTaken)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine($"\n{player.CurrentLocation.Item.Name}");
                Console.WriteLine(player.CurrentLocation.Item.Description);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("There's nothing noteworthy to examine here.");
            }
        }

        private void ShowOutro()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("╔══════════════════════════════════════════════════╗");
            Console.WriteLine("║                 G A M E   O V E R               ║");
            Console.WriteLine("╚══════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine("\nYour final stats:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Name: {player.Name}");
            Console.WriteLine($"Level: {player.Level}");
            Console.WriteLine($"Gold: {player.Gold}");
            Console.WriteLine($"Difficulty: {difficulty}");

            if (player.HasCrown)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nYou successfully retrieved the Cursed Crown!");
                Console.WriteLine("The kingdom will sing songs of your heroism for generations!");
            }
            else if (player.Health <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nYou died in your quest...");
                Console.WriteLine("The kingdom falls to darkness without the crown.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("\nYou abandoned your quest...");
                Console.WriteLine("The kingdom's fate remains uncertain.");
            }

            Console.ResetColor();
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey(true);
        }
    }

    enum Difficulty
    {
        Novice,
        Knight,
        Legend,
        Masochist
    }

    class Player
    {
        public string Name { get; }
        public int Health { get; set; }
        public int Gold { get; set; }
        public int XP { get; set; }
        public int Level { get; set; }
        public int XPToNextLevel { get; set; }
        public List<Item> Inventory { get; }
        public Weapon EquippedWeapon { get; set; }
        public Armor EquippedArmor { get; set; }
        public Location CurrentLocation { get; set; }
        public bool HasCrown { get; set; }
        private Difficulty Difficulty { get; }

        public Player(string name, Difficulty difficulty)
        {
            Name = name;
            Difficulty = difficulty;
            Health = 100;
            Gold = 20;
            XP = 0;
            Level = 1;
            XPToNextLevel = 100;
            Inventory = new List<Item>();
            EquippedWeapon = new Weapon("Rusty Dagger", "A basic weapon", 5);
            EquippedArmor = new Armor("Tattered Clothes", "Basic protection", 2);
            HasCrown = false;

            // Starting items based on difficulty
            switch (difficulty)
            {
                case Difficulty.Novice:
                    Inventory.Add(new Item("Health Potion", "Restores 30 health", ItemType.HealthPotion));
                    Inventory.Add(new Item("Health Potion", "Restores 30 health", ItemType.HealthPotion));
                    EquippedWeapon = new Weapon("Steel Sword", "A reliable weapon", 10);
                    EquippedArmor = new Armor("Leather Armor", "Sturdy protection", 5);
                    Gold = 50;
                    break;
                case Difficulty.Knight:
                    Inventory.Add(new Item("Health Potion", "Restores 30 health", ItemType.HealthPotion));
                    break;
                case Difficulty.Legend:
                    Health = 80; // Harder difficulty starts with less health
                    break;
                case Difficulty.Masochist:
                    Health = 50;
                    EquippedWeapon = new Weapon("Broken Stick", "Hardly a weapon", 2);
                    break;
            }
        }

        public void AddItem(Item item)
        {
            Inventory.Add(item);
        }

        public void RemoveItem(Item item)
        {
            Inventory.Remove(item);
        }

        public bool HasItem(Item item)
        {
            return Inventory.Contains(item);
        }

        public void AddXP(int amount)
        {
            XP += amount;
            if (XP >= XPToNextLevel)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;
            XP -= XPToNextLevel;
            XPToNextLevel = (int)(XPToNextLevel * 1.5);
            Health = 100 + (Level * 5);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nLEVEL UP! You are now level {Level}!");
            Console.WriteLine($"Health increased to {Health}!");
            Console.ResetColor();
        }
    }

    class World
    {
        public List<Location> Locations { get; }

        public World(Difficulty difficulty)
        {
            Locations = new List<Location>();
            InitializeLocations(difficulty);
            ConnectLocations();
        }

        private void InitializeLocations(Difficulty difficulty)
        {
            // Starting area
            var village = new Location("Village of Brightwood",
                "A peaceful village surrounded by lush forests. The villagers look worried.",
                null, null);

            var forest = new Location("Darkwood Forest",
                "A dense forest with twisted trees. The air feels heavy here.",
                Enemy.GenerateRandomEnemy(difficulty, 1), null);

            var caveEntrance = new Location("Cave Entrance",
                "A dark opening in the mountainside. Strange markings cover the walls.",
                Enemy.GenerateRandomEnemy(difficulty, 2),
                new Item("Torch", "Provides light in dark places", ItemType.Misc));

            var cave = new Location("Gloomy Cave",
                "Damp and dark, with the sound of dripping water echoing.",
                Enemy.GenerateRandomEnemy(difficulty, 3), null);

            var undergroundLake = new Location("Underground Lake",
                "A vast lake with glowing mushrooms providing eerie illumination.",
                null,
                new Item("Mysterious Key", "An old iron key with strange symbols", ItemType.Key));

            var ancientTemple = new Location("Ancient Temple",
                "A crumbling temple filled with ancient relics and traps.",
                Enemy.GenerateRandomEnemy(difficulty, 5),
                new Item("Ancient Sword", "A weapon from a forgotten age", ItemType.Weapon) { Power = 15 });

            var throneRoom = new Location("Throne Room",
                "A grand hall with a dark throne at its center. The Cursed Crown rests upon it.",
                new Enemy("Shadow King", 80, 20, 10, 500),
                new Item("Cursed Crown", "A crown radiating dark energy", ItemType.Crown));

            // Add all locations
            Locations.Add(village);
            Locations.Add(forest);
            Locations.Add(caveEntrance);
            Locations.Add(cave);
            Locations.Add(undergroundLake);
            Locations.Add(ancientTemple);
            Locations.Add(throneRoom);

            // Set required keys for certain locations
            ancientTemple.RequiredKey = undergroundLake.Item;
        }

        private void ConnectLocations()
        {
            // Village connections
            Locations[0].Exits.Add("e", Locations[1]); // Village -> Forest

            // Forest connections
            Locations[1].Exits.Add("w", Locations[0]); // Forest -> Village
            Locations[1].Exits.Add("n", Locations[2]); // Forest -> Cave Entrance

            // Cave Entrance connections
            Locations[2].Exits.Add("s", Locations[1]); // Cave Entrance -> Forest
            Locations[2].Exits.Add("e", Locations[3]); // Cave Entrance -> Cave

            // Cave connections
            Locations[3].Exits.Add("w", Locations[2]); // Cave -> Cave Entrance
            Locations[3].Exits.Add("d", Locations[4]); // Cave -> Underground Lake

            // Underground Lake connections
            Locations[4].Exits.Add("u", Locations[3]); // Underground Lake -> Cave
            Locations[4].Exits.Add("n", Locations[5]); // Underground Lake -> Ancient Temple

            // Ancient Temple connections
            Locations[5].Exits.Add("s", Locations[4]); // Ancient Temple -> Underground Lake
            Locations[5].Exits.Add("e", Locations[6]); // Ancient Temple -> Throne Room

            // Throne Room connections
            Locations[6].Exits.Add("w", Locations[5]); // Throne Room -> Ancient Temple
        }
    }

    class Location
    {
        public string Name { get; }
        public string Description { get; }
        public Enemy Enemy { get; set; }
        public Item Item { get; set; }
        public Dictionary<string, Location> Exits { get; }
        public Item RequiredKey { get; set; }

        public Location(string name, string description, Enemy enemy, Item item)
        {
            Name = name;
            Description = description;
            Enemy = enemy;
            Item = item;
            Exits = new Dictionary<string, Location>();
        }
    }

    class Enemy
    {
        public string Name { get; }
        public int Health { get; set; }
        public int Damage { get; }
        public int Defense { get; }
        public int XPReward { get; }
        public bool IsDefeated { get; set; }

        public Enemy(string name, int health, int damage, int defense, int xpReward)
        {
            Name = name;
            Health = health;
            Damage = damage;
            Defense = defense;
            XPReward = xpReward;
            IsDefeated = false;
        }

        public static Enemy GenerateRandomEnemy(Difficulty difficulty, int playerLevel)
        {
            Random random = new Random();
            string[] enemyTypes = { "Goblin", "Skeleton", "Bandit", "Wolf", "Orc", "Wraith" };
            string enemyType = enemyTypes[random.Next(enemyTypes.Length)];

            // Base stats adjusted by difficulty and player level
            int baseHealth = 30 + (playerLevel * 5);
            int baseDamage = 5 + playerLevel;
            int baseDefense = 2 + (playerLevel / 2);
            int baseXP = 50 + (playerLevel * 10);

            // Difficulty modifiers
            switch (difficulty)
            {
                case Difficulty.Novice:
                    baseHealth = (int)(baseHealth * 0.8);
                    baseDamage = (int)(baseDamage * 0.8);
                    break;
                case Difficulty.Legend:
                    baseHealth = (int)(baseHealth * 1.3);
                    baseDamage = (int)(baseDamage * 1.3);
                    baseDefense = (int)(baseDefense * 1.3);
                    break;
                case Difficulty.Masochist:
                    baseHealth = baseHealth * 2;
                    baseDamage = baseDamage * 2;
                    baseDefense = baseDefense * 2;
                    break;
            }

            // Random variation
            baseHealth += random.Next(-5, 10);
            baseDamage += random.Next(-1, 3);
            baseDefense += random.Next(-1, 2);

            return new Enemy($"{enemyType}", baseHealth, baseDamage, baseDefense, baseXP);
        }
    }

    class Item
    {
        public string Name { get; }
        public string Description { get; }
        public ItemType Type { get; }
        public bool IsTaken { get; set; }
        public int Power { get; set; } // For weapons/armor

        public Item(string name, string description, ItemType type)
        {
            Name = name;
            Description = description;
            Type = type;
            IsTaken = false;
            Power = 0;
        }

        public static Item GenerateRandomItem(Difficulty difficulty, int playerLevel)
        {
            Random random = new Random();
            ItemType[] commonTypes = { ItemType.HealthPotion, ItemType.Misc };
            ItemType[] rareTypes = { ItemType.Weapon, ItemType.Armor, ItemType.Key };

            // Determine if we should generate a rare item (20% chance)
            bool isRare = random.Next(1, 101) > 80;
            ItemType type;

            if (isRare)
            {
                type = rareTypes[random.Next(rareTypes.Length)];
            }
            else
            {
                type = commonTypes[random.Next(commonTypes.Length)];
            }

            string name = "";
            string description = "";
            int power = 0;

            switch (type)
            {
                case ItemType.HealthPotion:
                    name = "Health Potion";
                    description = "Restores 30 health";
                    break;
                case ItemType.Weapon:
                    string[] weapons = { "Sword", "Axe", "Mace", "Dagger", "Spear" };
                    string weaponType = weapons[random.Next(weapons.Length)];
                    name = $"{GetQualityPrefix()} {weaponType}";
                    description = $"A {weaponType.ToLower()} of {GetQualityDescription()} quality";
                    power = 5 + playerLevel + random.Next(0, 5);
                    break;
                case ItemType.Armor:
                    string[] armors = { "Helmet", "Chestplate", "Gauntlets", "Boots", "Shield" };
                    string armorType = armors[random.Next(armors.Length)];
                    name = $"{GetQualityPrefix()} {armorType}";
                    description = $"A {armorType.ToLower()} of {GetQualityDescription()} quality";
                    power = 2 + (playerLevel / 2) + random.Next(0, 3);
                    break;
                case ItemType.Key:
                    name = "Mysterious Key";
                    description = "It might unlock something important";
                    break;
                case ItemType.Misc:
                    string[] misc = { "Torch", "Rope", "Herbs", "Scroll", "Gem" };
                    name = misc[random.Next(misc.Length)];
                    description = $"A {name.ToLower()} that might be useful";
                    break;
            }

            return new Item(name, description, type) { Power = power };
        }

        private static string GetQualityPrefix()
        {
            Random random = new Random();
            string[] prefixes = { "Rusty", "Steel", "Silver", "Golden", "Enchanted", "Ancient" };
            return prefixes[random.Next(prefixes.Length)];
        }

        private static string GetQualityDescription()
        {
            Random random = new Random();
            string[] qualities = { "poor", "average", "good", "excellent", "masterwork" };
            return qualities[random.Next(qualities.Length)];
        }
    }

    class Weapon : Item
    {
        public Weapon(string name, string description, int power)
            : base(name, description, ItemType.Weapon)
        {
            Power = power;
        }
    }

    class Armor : Item
    {
        public Armor(string name, string description, int power)
            : base(name, description, ItemType.Armor)
        {
            Power = power;
        }
    }

    enum ItemType
    {
        HealthPotion,
        Weapon,
        Armor,
        Key,
        Crown,
        Misc
    }

    class Combat
    {
        private Player player;
        private Enemy enemy;
        private Random random = new Random();

        public Combat(Player player, Enemy enemy)
        {
            this.player = player;
            this.enemy = enemy;
        }

        public void Start()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nCombat begins! You face the {enemy.Name}!");
            Console.ResetColor();

            while (player.Health > 0 && enemy.Health > 0)
            {
                DisplayCombatStatus();
                PlayerTurn();

                if (enemy.Health <= 0)
                {
                    EnemyDefeated();
                    return;
                }

                EnemyTurn();

                if (player.Health <= 0)
                {
                    return; // Player defeated
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey(true);
            }
        }

        private void DisplayCombatStatus()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n{player.Name}");
            Console.WriteLine($"Health: {player.Health}/100");
            Console.WriteLine($"Weapon: {player.EquippedWeapon.Name} (DMG: {player.EquippedWeapon.Power})");
            Console.WriteLine($"Armor: {player.EquippedArmor.Name} (DEF: {player.EquippedArmor.Power})");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{enemy.Name}");
            Console.WriteLine($"Health: {enemy.Health}");
            Console.ResetColor();
        }

        private void PlayerTurn()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nYour turn! (Attack, Use Item, Flee)");
            Console.ResetColor();

            string input = Console.ReadLine().ToLower();

            if (input == "attack")
            {
                int damage = CalculatePlayerDamage();
                enemy.Health -= damage;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"You hit the {enemy.Name} for {damage} damage!");
                Console.ResetColor();
            }
            else if (input.StartsWith("use "))
            {
                string itemName = input.Substring(4);
                Item item = player.Inventory.Find(i => i.Name.ToLower() == itemName.ToLower());

                if (item != null && item.Type == ItemType.HealthPotion)
                {
                    player.Health = Math.Min(100, player.Health + 30);
                    player.RemoveItem(item);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"You drink the {item.Name} and restore 30 health!");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("You can't use that in combat!");
                }
            }
            else if (input == "flee")
            {
                if (random.Next(1, 101) > 50) // 50% chance to flee
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("You successfully flee from combat!");
                    Console.ResetColor();
                    enemy.Health = 0; // End combat
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You failed to flee!");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.WriteLine("Invalid command! You hesitate and lose your chance to act.");
            }
        }

        private int CalculatePlayerDamage()
        {
            int baseDamage = player.EquippedWeapon.Power;
            int variation = random.Next(-2, 3); // -2 to +2 variation
            int totalDamage = baseDamage + variation;

            // Critical hit chance (15%)
            if (random.Next(1, 101) > 85)
            {
                totalDamage *= 2;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Critical hit!");
                Console.ResetColor();
            }

            // Account for enemy defense
            totalDamage -= enemy.Defense / 2;
            return Math.Max(1, totalDamage); // Always do at least 1 damage
        }

        private void EnemyTurn()
        {
            int damage = CalculateEnemyDamage();
            player.Health -= damage;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"The {enemy.Name} hits you for {damage} damage!");
            Console.ResetColor();
        }

        private int CalculateEnemyDamage()
        {
            int baseDamage = enemy.Damage;
            int variation = random.Next(-1, 2); // -1 to +1 variation
            int totalDamage = baseDamage + variation;

            // Account for player armor
            totalDamage -= player.EquippedArmor.Power / 2;
            return Math.Max(1, totalDamage); // Always do at least 1 damage
        }

        private void EnemyDefeated()
        {
            enemy.IsDefeated = true;
            int xpReward = enemy.XPReward;
            int goldReward = random.Next(5, 20) * player.Level;

            player.AddXP(xpReward);
            player.Gold += goldReward;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nYou defeated the {enemy.Name}!");
            Console.WriteLine($"Gained {xpReward} XP and {goldReward} gold!");
            Console.ResetColor();

            if (player.XP >= player.XPToNextLevel)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nYou feel stronger! (Level up!)");
                Console.ResetColor();
            }
        }
    }
}
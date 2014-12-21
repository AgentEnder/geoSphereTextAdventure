using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TextAdventure
{
    public class mainGame
    {
        static List<item> craftableGameItems;
        static List<action> actionList = new List<action>();
        static List<structure> structures = new List<structure>();
        //Initialize Structures
        static structure altar = new structure();
        static structure mine = new structure();
        //Initialize Items
        static item smallStone = new item();
        static item plantFiber = new item();
        static item rope = new item();
        static item stone = new item();
        static item stick = new item();
        static item snow = new item();
        static item snowball = new item();
        static item stoneAxeHead = new item();
        static item axeHandle = new item();
        static item stoneAxe = new item();
        static item wood = new item();
        static item woodPlank = new item();
        static item sand = new item();
        static item sandstone = new item();
        static item cactus = new item();
        //Unlockable Actions
        static action useAxe = new action();
        static action build = new action();

        static Random rand = new Random();

        public static void unlockAction(action a, Player p)
        {
            if (p.actions.Contains(a))
            {
                Console.WriteLine("Player Already has action, fix your code dmn it");
            }
            else
            {
                p.actions.Add(a);
            }
            p.actions.Sort((z, q) => z.order.CompareTo(q.order));
        }
        public static string getInput()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            string input = Console.ReadLine().ToLower().Trim();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            return (input);
        }
        public static string evaluateActions(Player player, string action, biome Biome)
        {
            string evaluatedAction = action;
            //Check structures for effects
            int chanceOfEncounter = 7;
            int chanceOfEncounterMining = chanceOfEncounter;
            int forageChanceBoost = 0;
            foreach (var x in player.structuresBuilt)
            {
                if(x.location == Biome)
                {
                    if (x.effect == 1 || x.effect == 1.1)
                    {
                        if (x.effect == 1.1)
                        {
                            if (chanceOfEncounterMining - x.strength > 0)
                            {
                                chanceOfEncounterMining = chanceOfEncounterMining - x.strength;
                            }
                            else
                            {
                                chanceOfEncounterMining = 0;
                            }
                        }
                        else
                        {
                            if (chanceOfEncounter - x.strength > 0)
                            {
                                chanceOfEncounter = chanceOfEncounter - x.strength;
                            }
                            else
                            {
                                chanceOfEncounter = 0;
                            }
                        }
                    }
                    else
                    {
                        if (x.effect == 2)
                        {
                            forageChanceBoost += x.strength;
                        }
                    }
                }
            }
            if (chanceOfEncounter < chanceOfEncounterMining)
            {
                chanceOfEncounterMining = chanceOfEncounter;
            }
            //Evaluate actions
            switch (action.ToLower())
            {
                case "debugcheats":
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            player.give(wood);
                            player.give(stone);
                            player.give(rope);
                            player.give(stoneAxe);
                            
                        }
                        unlockAction(useAxe, player);
                    }break;
                case "build a structure":
                case "build":
                    {
                        List<structure> buildables = new List<structure>();
                        foreach (var build in structures)
                            //Console.WriteLine("{0} x {1}",x.name , player.inventory.Count(item => x.name.Equals(item.name)));
                        {
                            bool canBuild = true;
                            List<item> reqMat = new List<item>();
                            foreach(var x in build.reqMat)
                            {
                                if (!reqMat.Contains(x))
                                {
                                    reqMat.Add(x);
                                }
                            }
                            foreach (var x in reqMat)
                            {
                                if(build.reqMat.Count(req => x.name.Equals(req.name)) > player.inventory.Count(req => x.name.Equals(req.name)))
                                {
                                    canBuild = false;
                                }
                            }
                            if (canBuild)
                            {
                                buildables.Add(build);
                            }
                           
                        }
                        if (!buildables.Any())
                        {
                            Console.WriteLine("You don't have the required materials to build anything!\r\n");
                        }
                        else
                        {
                            Console.WriteLine("Which would you like to build: ");
                            foreach (var x in buildables)
                            {
                                Console.WriteLine(x.name);
                            }
                            string buildStructure = getInput();
                            bool hasStruct = false;
                            structure building = new structure();
                            foreach (var x in buildables)
                            {
                                if (x.name.ToLower() == buildStructure.ToLower()) { hasStruct = true; building = x; }
                            }
                            if (hasStruct)
                            {
                                foreach (var x in building.reqMat)
                                {
                                    Console.WriteLine("removing {0}", x.name);
                                    player.inventory.Remove(x);
                                }
                                building.location = Biome;
                                player.structuresBuilt.Add(building);
                            }
                            building = null;
                        }
                    }break;
                case "use axe":
                    {
                        Console.WriteLine("How would you like to use your axe?");
                        if (Biome.resources.Contains(wood))
                        {
                            Console.WriteLine("cut down a tree? (cut)");
                        }
                        else if(Biome.resources.Contains(cactus))
                        {
                            Console.WriteLine("cut down a cacti? (cut)");
                        }
                        else
                        {
                            Console.WriteLine("There is nothing to cut down!");
                        }
                        string choice = getInput();
                        switch (choice.ToLower())
                        {
                            case "cut":
                            case "cut down a tree":
                                {
                                    foreach (var x in Biome.resources)
                                    {
                                        if (x.cuttable == true)
                                        {
                                            Random rng = new Random();
                                            for (int i = 0; i < rng.Next(3, 7); i++)
                                            {
                                                player.inventory.Add(x);
                                            }
                                        }
                                    }
                                    
                                }break;
                            default:
                                {

                                }break;
                        }
                    }break;
                case "forage":
                case "forage for resources":
                case "forage for items":
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            foreach (var x in Biome.resources)
                            {
                                int chance = rand.Next(0, 100);
                                if (Biome.chanceOfGettingResource > (chance-forageChanceBoost) & x.forageable == true)
                                {
                                    player.inventory.Add(x);
                                    Console.WriteLine("You found a {0}", x.name);
                                }
                            }
                        }
                        Console.WriteLine("\r\n");
                    }break;
                
                case "look around":
                    {
                        Console.WriteLine(Biome.description);
                    }break;
                case "pick up dropped items":
                    {
                        foreach (var x in Biome.itemsDropped)
                        {
                            Console.WriteLine("\r\n Which item would you like to pick up?");
                            Console.WriteLine(x.name);
                            string itemName = getInput();
                            if (x.name == itemName)
                            {
                                player.inventory.Add(x);
                                Console.WriteLine("Sucessfully picked up item");
                            }
                            else
                            {
                                Console.WriteLine("No such item was dropped!");
                            }
                        }
                    }break;
                case "access inventory":
                case "inventory":
                    {
                        player.inventory.Sort((p, q) => p.name.CompareTo(q.name));
                        Console.WriteLine("Your inventory contains:");
                        List<item> inventoryItems = new List<item>();
                        foreach (var x in player.inventory)
                        {
                            if (!inventoryItems.Contains(x))
                            {
                                inventoryItems.Add(x);
                            }
                        }
                        foreach (var x in inventoryItems)
                        {
                            Console.WriteLine("{0} x {1}",x.name , player.inventory.Count(item => x.name.Equals(item.name)));
                        }
                        Console.Write("\r\n What would you like to use? or would you like to return\r\n");
                        string inventoryAction = getInput();
                        item inventoryItem = new item();
                        foreach (var x in player.inventory)
                        {
                            if (x.name == inventoryAction)
                            {
                                inventoryItem = x;
                            }
                        }
                        if (player.inventory.Contains(inventoryItem)) { player.inventory.Remove(inventoryItem); }
                        if (inventoryItem.name == "stone" || inventoryItem.name == "stick")
                        {
                            Console.WriteLine("You have dropped a {0}\r\n", inventoryItem.name);
                            Biome.itemsDropped.Add(inventoryItem);
                        }
                        
                        evaluatedAction = "inventory";
                    }break;
                case "crafting":
                case "craft":
                    {
                        bool crafting = false;
                        List<item> craftables = new List<item>();
                        foreach (var x in craftableGameItems)
                        {
                            bool craftable = true;
                            int numberOfIngredients;
                            numberOfIngredients = x.craftingIngredients.Count();
                            for (int i = 0; i < numberOfIngredients; i++)
                            {
                                if(!player.inventory.Contains(x.craftingIngredients[i]))
                                {
                                    craftable = false;
                                }
                            }
                            if (craftable)
                            {
                                craftables.Add(x);
                            }
                        }
                        Console.WriteLine("\r\n What would you like to craft?");
                        foreach (var x in craftables)
                        {
                            Console.WriteLine(x.name);
                        }
                        string craftItem = getInput();
                        Console.Write("\r\n");
                        foreach (var x in craftables)
                        {
                            if(x.name.ToLower() == craftItem.ToLower())
                            {
                                int numberOfIngredients = x.craftingIngredients.Count();
                                for (int i = 0; i < numberOfIngredients; i++)
                                {
                                    player.inventory.Remove(x.craftingIngredients[i]);
                                }
                                for (int i = 0; i < x.craftingQuantity; i++)
                                {
                                    player.inventory.Add(x);
                                    if (x.type == 1)
                                    {
                                        if (!player.actions.Contains(useAxe))
                                        {
                                            unlockAction(useAxe, player);
                                        }
                                    }
                                    crafting = true;
                                }
                            }
                        }
                        if (crafting) 
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("CRAFTING");
                            Thread.Sleep(500); Console.Write("-");
                            Thread.Sleep(500); Console.Write("-");
                            Thread.Sleep(500); Console.Write("-");
                            Thread.Sleep(500); Console.Write("-\r\n");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.WriteLine("Unable to craft {0} \r\n", craftItem);
                        }

                    }break;
                case "leave":
                    {
                        evaluatedAction = "leave";
                    }break;
                case "exit":
                    {
                        Environment.Exit(0);
                    }break;
                default:
                    {
                        Console.WriteLine("Unknown Action");
                    }break;
            }
            bool inventoryHasBuilding = false;
            foreach (var x in player.inventory)
            {
                if (x.type == 10 & x != stone)
                {
                    inventoryHasBuilding = true;
                }
            }
            if (inventoryHasBuilding == true & !player.actions.Contains(mainGame.build))
            {
                unlockAction(build, player);
            }

            player.actions.Sort((p, q) => p.title.CompareTo(q.title));
            return (evaluatedAction);
        }
        public static void Main()
        {
            #region ItemInitializationAndCrafting
            //initialize item atributes
            
            smallStone.name = "small stone";
            smallStone.forageable = true;
            plantFiber.name = "Plant Fiber";
            plantFiber.forageable = true;
            rope.name = "Rope";
            stone.name = "stone";
            stone.type = 10;
            stick.name = "stick";
            stick.forageable = true;
            snow.name = "snow";
            snowball.name = "snowball";
            snowball.forageable = true;
            stoneAxeHead.name = "Stone Axe Head";
            axeHandle.name = "Axe Handle";
            stoneAxe.name = "Stone Axe";
            stoneAxe.type = 1;
            wood.name = "wood";
            wood.type = 10;
            wood.cuttable = true;
            woodPlank.name = "Wooden Plank";
            woodPlank.type = 10;
            sand.name = "sand";
            sand.forageable = true;
            sandstone.name = "sandstone";
            sandstone.type = 10;
            cactus.name = "cactus";
            cactus.forageable = false;
            cactus.cuttable = true;
            //crafting
            craftableGameItems = new List<item>();
            woodPlank.craftingIngredients = new List<item>();
            woodPlank.craftingIngredients.Add(wood);
            woodPlank.craftingQuantity = 4;
            craftableGameItems.Add(woodPlank);
            rope.craftingIngredients = new List<item>();
            rope.craftingIngredients.Add(plantFiber);
            rope.craftingIngredients.Add(plantFiber);
            rope.craftingQuantity = 1;
            craftableGameItems.Add(rope);
            stoneAxeHead.craftingIngredients = new List<item>();
            stoneAxeHead.craftingIngredients.Add(stone);
            stoneAxeHead.craftingIngredients.Add(stone);
            stoneAxeHead.craftingIngredients.Add(stone);
            stoneAxeHead.craftingQuantity = 1;
            craftableGameItems.Add(stoneAxeHead);
            axeHandle.craftingIngredients = new List<item>();
            axeHandle.craftingIngredients.Add(stick);
            axeHandle.craftingIngredients.Add(stick);
            axeHandle.craftingIngredients.Add(plantFiber);
            axeHandle.craftingQuantity = 1;
            craftableGameItems.Add(axeHandle);
            stoneAxe.craftingIngredients = new List<item>();
            stoneAxe.craftingIngredients.Add(stoneAxeHead);
            stoneAxe.craftingIngredients.Add(axeHandle);
            stoneAxe.craftingIngredients.Add(rope);
            stoneAxe.craftingQuantity = 1;
            craftableGameItems.Add(stoneAxe);
            snow.craftingIngredients = new List<item>();
            snow.craftingIngredients.Add(snowball);
            snow.craftingIngredients.Add(snowball);
            snow.craftingIngredients.Add(snowball);
            snow.craftingQuantity = 1;
            craftableGameItems.Add(snow);
            snowball.craftingIngredients = new List<item>();
            snowball.craftingIngredients.Add(snow);
            snowball.craftingQuantity = 1;
            craftableGameItems.Add(snowball);
            stone.craftingIngredients = new List<item>();
            stone.craftingIngredients.Add(smallStone);
            stone.craftingIngredients.Add(smallStone);
            stone.craftingIngredients.Add(smallStone);
            stone.craftingQuantity = 1;
            craftableGameItems.Add(stone);
            sandstone.craftingIngredients = new List<item>();
            sandstone.craftingIngredients.Add(sand);
            sandstone.craftingIngredients.Add(sand);
            sandstone.craftingIngredients.Add(sand);
            sandstone.craftingIngredients.Add(sand);
            sandstone.craftingQuantity = 1;
            craftableGameItems.Add(sandstone);
            foreach (var x in craftableGameItems)
            {
                if (x.craftingResults == null)
                {
                    List<item> results = new List<item>();
                    results.Add(x);
                }
            }
            #endregion
            //Initialize structure attributes
            altar.name = "Altar";
            altar.effect = 2;
            altar.strength = 3;
            altar.reqMat = new List<item>();
            for (int i = 0; i < 5; i++)
            {
                altar.reqMat.Add(wood);
            }
            for (int i = 0; i < 10; i++)
            {
                altar.reqMat.Add(stone);
                altar.reqMat.Add(rope);
            }
            structures.Add(altar);
            mine.name = "Mine";
            mine.effect = 1.1;
            mine.strength = 5;
            mine.reqMat = new List<item>();
            for (int i = 0; i < 50; i++)
            {
                mine.reqMat.Add(stone);
            }
            for (int i = 0; i < 15; i++)
            {
                mine.reqMat.Add(wood);
            }
            structures.Add(mine);
            //Initialize Biomes
            List<biome> biomes = new List<biome>();
            //Taiga
            biome taiga = new biome();
            taiga.name = "taiga";
            taiga.description = "\r\n As you wander about the taiga you shiver from the cold, looking around at the various possible resources \r\n";
            taiga.chanceOfGettingResource = 20;
            taiga.resources = new List<item>();
            taiga.resources.Add(stick);
            taiga.resources.Add(plantFiber);
            taiga.resources.Add(plantFiber);
            taiga.resources.Add(snowball);
            taiga.resources.Add(wood);
            taiga.resources.Add(smallStone);
            taiga.itemsDropped = new List<item>();
            biomes.Add(taiga);
            //Desert
            biome desert = new biome();
            desert.name = "desert";
            desert.description = "\r\n Looking across the oceans of sand you see a few measely cacti and stone, nothing else among the horizon \r\n ";
            desert.resources = new List<item>();
            desert.resources.Add(sand);
            desert.resources.Add(sand);
            desert.resources.Add(sand);
            desert.resources.Add(sand);
            desert.resources.Add(smallStone);
            desert.resources.Add(smallStone);
            desert.resources.Add(smallStone);
            desert.resources.Add(smallStone);
            desert.resources.Add(cactus);
            desert.resources.Add(cactus);
            desert.resources.Add(stick);
            desert.resources.Add(plantFiber);
            desert.chanceOfGettingResource = 5;
            desert.itemsDropped = new List<item>();
            biomes.Add(desert);

            biome currentBiome = new biome();

            #region InitializePlayerAndActions
            //Initialize Player
            Console.WriteLine("Hello, welcome to the game\r\n");
            Console.WriteLine("What is your name, Hero?");
            Player player = new Player();
            //Player Attributes
            player.maxHealth = 20;
            player.health = player.maxHealth;
            player.strength = 5;
            player.armor = 0;
            player.name = getInput();
            //Player Inventory
            player.inventory = new List<item>();
            player.inventory.Add(stone);
            player.inventory.Add(stick);
            //Player Structure
            player.structuresBuilt = new List<structure>();
            //Initialize Actions
            action look = new action();
            look.title = "look around";
            look.order = 1;
            action forage = new action();
            forage.title = "forage for resources";
            forage.order = 2;
            action droppedItems = new action();
            droppedItems.title = "Pick up dropped items";
            droppedItems.order = 3;
            action inventory = new action();
            inventory.title = "access inventory";
            inventory.order = 4;
            action crafting = new action();
            crafting.title = "crafting";
            crafting.order = 5;
            //useAxe
            useAxe.title = "Use Axe";
            useAxe.order = 6;
            //Build
            build.title = "Build Structure";
            build.order = 7;
            action leave = new action();
            leave.title = "leave area";
            leave.order = 98;
            action exit = new action();
            exit.title = "exit";
            exit.order = 99;
            actionList.Add(useAxe);
            //Player Actions
            player.actions = new List<action>();
            player.actions.Add(look);
            player.actions.Add(forage);
            player.actions.Add(droppedItems);
            player.actions.Add(inventory);
            player.actions.Add(crafting);
            player.actions.Add(leave);
            player.actions.Add(exit);
            #endregion

            //Start Game
            Console.WriteLine("Welcome {0} to the geoSphere, home of various biomes.\r\n", player.name);
            
            
            bool running = true ;
            bool inBattle = false;
            
            //Game Loop
            while(running)
            {
                Console.WriteLine("Which biome would you like to visit? your choices are: ");
                foreach (var x in biomes)
                {
                    Console.WriteLine(x.name);
                }
                Console.Write("\r\n");
                bool correctChoice = false;
                
                while (correctChoice == false)
                {
                    string biomeName = getInput();
                    if (biomeName == "exit")
                    {
                        evaluateActions(player, "exit", new biome());
                    }
                    foreach (var x in biomes)
                    {
                        if (x.name == biomeName)
                        {
                            currentBiome = x;
                            correctChoice = true;
                        }
                    }
                }
                while (true)
                {
                    Console.WriteLine("What would you like to do in the {0}?", currentBiome.name);
                    player.actions.Sort((z, q) => z.order.CompareTo(q.order));
                    foreach (var x in player.actions)
                    {
                        Console.WriteLine(x.title);    
                    }
                    Console.Write("\r\n");
                    string action = evaluateActions(player, getInput(), currentBiome);
                    //Console.WriteLine("Thank you for visiting the taiga -geoSphere Management");
                    //biomes.Remove("taiga");
                    if (action == "leave current area" || action == "leave" || action == "travel")
                    {
                        break;
                    }
                }
                if(inBattle)
                {
                }
                
            }
            System.Console.ReadKey();
        }
    }
    /// <summary>
    /// Classes for biomes, items, players, enemies, and battles
    /// </summary>
    public class action
    {
        public string title;
        public int order;
    }
    public class structure
    {
        public string name;
        public double effect; //1 = lower chance of encounter, 1.1 = lower chance while mining, 1.2 = lower chance while foraging, 2 = increase forage chance
        public int strength;
        public biome location;
        public List<item> reqMat;
    }
    public class biome
    {
        public string name;
        public string description;
        public List<item> resources;
        public int chanceOfGettingResource;
        public List<item> itemsDropped;
    }
    public class item
    {
        public string name;
        public List<string> effects;
        public int health;
        public bool forageable;
        public int craftingQuantity;
        public List<item> craftingResults;
        public int type; //null = unspecified, 1 = Axe, 2 = PickAxe, 3 = Shovel, 4 = Sword, 10 = Building Mat
        public List<item> craftingIngredients;
        public bool cuttable;
    }
    public class Player
    {
        public string name;
        public List<item> inventory;
        public List<action> actions;
        public List<structure> structuresBuilt;
        public int health;
        public int maxHealth;
        public int strength;
        public int armor;

        public void give(item a)
        {
            inventory.Add(a);
        }
        public void give(item a, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                inventory.Add(a);   
            }
        }
    }

    public class enemy
    {
        public int dmgMin;
        public int dmgMax;
        public int healthMax;
        public int health;
        public int type;
    }

    public class battle
    {
        public Player player; 
        public int enemyType;
        public int enemyCount;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTG_calculator
{
    static public class Program
    {
        #region Variables
        static int[] cardsInSet;
        static int[] wildcardRouletTimer; // The timer information for wildcards guaranteed per pack
        static int[] randomWildcardRouletTimer; // The timer information for wildcards randomly gained
        static double[] cardsInPacks; // The cards we gained from packs. Starts at 1 pack and is increased later
        static double[] openedWildcards;
        static string[] rarityNames = new string[] { "common", "uncommon", "rare", "mythic rare" };

        static int[,] cardsToOpen; // For a deck per set
        static int[,] cardsForcedToCraft;
        static double[,] cardCopiesOwned; // Only an average

        static int packsAlreadyOpened;
        static public List<PacksForDecksDictionary> packsForDecks = new List<PacksForDecksDictionary>(); // See class comment
        static public List<PacksForSetsDictionary> packsPerSets = new List<PacksForSetsDictionary>(); // See class comment
        static int packsToOpen;
        static int maxDuplicates; // 4
        static int rarityAmount; // 4
        static int craftGoal; // Wildcards per rarity
        static double craftProgress; // Wildcards per rarity
        static string set;
        static string deck;
        static double vaultPoints = 0; // MTG mechanic

        // find reprints in Scryfall.com
        // (set:M21 (in:iko or in:thb or in:eld or in:m20 or in:war or in:rna or in:grn)) or (in:iko (in:thb or in:eld or in:m20 or in:war or in:rna or in:grn)) or (in:thb (in:eld or in:m20 or in:war or in:rna or in:grn)) or (in:eld (in:m20 or in:war or in:rna or in:grn)) or (in:m20 (in:war or in:rna or in:grn)) or (in:m20 (in:war or in:rna or in:grn)) or (in:war (in:rna or in:grn)) or (in:rna in:GRN)
        static string[] setMemory = new string[] { "ZNR", "M21", "IKO", "THB", "ELD", "M20", "WAR", "RNA", "GRN", "M19", "DOM", "RIX", "XLN", "AKR" };

        // Here the names of the decks are inserted. If you want to skip a deck in the calculation, but feel the need to use it later, you can comment it here.
        // Data will be kept, but it will not be taken into the calculations.
        static List<string> deckMemory = new List<string> {
            // Standard
            "WUBRG Plants 1IKO 3M21",
            //"WUBRG Plants 2M20 3M21",
            //"WUBRG Plants 1RIX 3M21",
            //"WUBRG Plants 1AKR 3M21",
            //"WUBRG Plants 1IKO 3M20",
            //"WUBRG Plants 2M20 3M20",
            //"WUBRG Plants 1RIX 3M20",
            //"WUBRG Plants 1AKR 3M20",

            "Selesnya Counters",

            // Life land
            //"Rakdos Party Aggro M21",
            //"Rakdos Party Aggro IKO",
            "Rakdos Party Aggro M20",

            // Temple - Life land
            "Boros Warriors 1M21 2M21",
            "Boros Warriors 1M21 2IKO",
            "Boros Warriors 1M21 2ELD",
            "Boros Warriors 1M21 2M20",
            "Boros Warriors 1M20 2M21",
            "Boros Warriors 1M20 2IKO",
            "Boros Warriors 1M20 2ELD",
            "Boros Warriors 1M20 2M20",

            // Temple - Life land
            //"Simic Scute 1M21 2M21",
            //"Simic Scute 1M21 2IKO",
            //"Simic Scute 1M21 2ELD",
            //"Simic Scute 1M21 2M20",
            //"Simic Scute 1M20 2M21",
            //"Simic Scute 1M20 2IKO",
            //"Simic Scute 1M20 2ELD",
            //"Simic Scute 1M20 2M20",
            //"Simic Scute 2M21",
            //"Simic Scute 2IKO",
            //"Simic Scute 2ELD",
            //"Simic Scute 2M20",

            // Temple - Life land
            //"Simic Kicker 1M21 2M21",
            //"Simic Kicker 1M21 2IKO",
            "Simic Kicker 1M21 2ELD",
            //"Simic Kicker 1M21 2M20",
            //"Simic Kicker 1M20 2M21",
            //"Simic Kicker 1M20 2IKO",
            //"Simic Kicker 1M20 2ELD",
            //"Simic Kicker 1M20 2M20",

            // Historic
            //"Tempered Steel 1ZNR",
            //"Tempered Steel 1M21",
            //"Tempered Steel 1IKO",
            //"Tempered Steel 1THB",
            //"Tempered Steel 1ELD",
            //"Tempered Steel 1M20",
            //"Tempered Steel 1WAR",
            //"Tempered Steel 1RNA",
            //"Tempered Steel 1GRN",
            //"Tempered Steel 1M19",
            "Tempered Steel 1DOM",
            //"Tempered Steel 1RIX",
            //"Tempered Steel 1XLN",
            //"Tempered Steel 1AKR",
            //"Tempered Steel 2ZNR",
            //"Tempered Steel 2M21",
            //"Tempered Steel 2IKO",
            //"Tempered Steel 2THB",
            //"Tempered Steel 2ELD",
            //"Tempered Steel 2M20",
            //"Tempered Steel 2WAR",
            //"Tempered Steel 2RNA",
            //"Tempered Steel 2GRN",
            //"Tempered Steel 2M19",
            //"Tempered Steel 2DOM",
            //"Tempered Steel 2RIX",
            //"Tempered Steel 2XLN",
            //"Tempered Steel 2AKR"
        };

        // The difference between the two following dictionaries is that packsPerSetsDictionary remembers for EACH SET how many packs were opened for AN ENTIRE DECK 
        
        // while packsForDecksDictionary is a bit more complicated. This dictionary only remembers how many cards for A SINGLE SET it needs to open (Value) and with
        // how many packs this should be TESTED with.

        // PacksForSetsDictionary is meant to remember info while PacksForDecksDictionary is meant to be tested with (in the case of ordering decks from cheapest to most
        // expensive)

        // A variable that is used to simulate a dictionary with duplicate keys
        public class PacksForSetsDictionary
        {
            public string key;
            public int[] value;

            public PacksForSetsDictionary(string key, int[] value)
            {
                this.key = key;
                this.value = value;
            }

            public string Key
            {
                get
                {
                    return key;
                }
            }
            public int[] Value
            {
                get
                {
                    return value;
                }
            }
        }

        // A variable that is used to simulate a dictionary with duplicate keys
        public class PacksForDecksDictionary
        {
            public int key;
            public int[,] value;
            public int[,] forcedValue;

            public PacksForDecksDictionary(int key, int[,] value, int[,] forcedValue)
            {
                this.key = key;
                this.value = value;
                this.forcedValue = forcedValue;
            }

            public int Key
            {
                get
                {
                    return key;
                }
            }
            public int[,] Value
            {
                get
                {
                    return value;
                }
            }
            public int[,] ForcedValue
            {
                get
                {
                    return forcedValue;
                }
            }
        }
        #endregion

        // Starting point
        static void Main(string[] args)
        {
            // Check if the client wants to check for the cheapest decks
            Console.Write("Do you want to figure out the cheapest set? Click y: ");
            bool checkAllDecks = Console.ReadKey().Key == ConsoleKey.Y;

            Console.WriteLine();

            // If we only want to check how many cards we get from packs, do the following.
            if (!checkAllDecks)
            {
                // Ask for set
                Console.Write("What set: ");
                set = Console.ReadLine();

                // Ask for packs
                Console.Write("How many packs to open: ");
                packsToOpen = Convert.ToInt32(Console.ReadLine());

                // Here, decks can be inserted with a pack value to also add the average crafted cards into the calculation per crafted deck. 
                // Recommended to figure out with CalculateCheapestDecks what these values are and insert them here. Be careful to not respond with a lower number in the 
                // question above, as weird results end up happening so comment again if this problem occurs. Sort these from cheapest to most expensive.
                #region Add average crafted cards per crafted deck
                deck = "Template";
                AddDeck(0);

                switch (set)
                {
                    case "ZNR":
                        #region ZNR
                        deck = "Tempered Steel DOM";
                        AddDeck(0);

                        deck = "Rakdos Party Aggro M20";
                        AddDeck(29);

                        deck = "Simic Kicker 1M21 2M21";
                        AddDeck(72);

                        deck = "Simic Scute 2IKO";
                        AddDeck(84);

                        deck = "All Commons ZNR";
                        AddDeck(104);

                        deck = "WUBRG Plants";
                        AddDeck(126);
                        #endregion
                        break;
                    case "M21":
                        #region M21
                        deck = "Tempered Steel DOM";
                        AddDeck(0);

                        deck = "Rakdos Party Aggro M20";
                        AddDeck(12);

                        deck = "Simic Kicker M21";
                        AddDeck(21);

                        deck = "Simic Scute 2IKO";
                        AddDeck(21);

                        deck = "WUBRG Plants";
                        AddDeck(42);
                        #endregion
                        break;
                    case "IKO":
                        #region IKO
                        deck = "Tempered Steel DOM";
                        AddDeck(0);

                        deck = "Rakdos Party Aggro M20";
                        AddDeck(12);

                        deck = "Simic Kicker 1M21 2M21";
                        AddDeck(12);

                        deck = "Simic Scute 2IKO";
                        AddDeck(44);

                        deck = "WUBRG Plants";
                        AddDeck(52);
                        #endregion
                        break;
                    case "THB":
                        #region THB
                        deck = "Tempered Steel DOM";
                        AddDeck(0);

                        deck = "Rakdos Party Aggro M20";
                        AddDeck(25);

                        deck = "Simic Kicker 1M21 2M21";
                        AddDeck(25);

                        deck = "Simic Scute 2IKO";
                        AddDeck(25);

                        deck = "WUBRG Plants";
                        AddDeck(25);
                        #endregion
                        break;
                    case "ELD":
                        #region ELD
                        deck = "Tempered Steel DOM";
                        AddDeck(21);

                        deck = "Rakdos Party Aggro M20";
                        AddDeck(25);

                        deck = "Simic Kicker 1M21 2M21";
                        AddDeck(25);

                        deck = "Simic Scute 2IKO";
                        AddDeck(48);

                        deck = "WUBRG Plants";
                        AddDeck(48);
                        #endregion
                        break;
                    case "M20":
                        #region M20
                        deck = "Tempered Steel DOM";
                        AddDeck(21);
                        #endregion
                        break;
                    case "WAR":
                        #region WAR
                        deck = "Tempered Steel DOM";
                        AddDeck(0);
                        #endregion
                        break;
                    case "RNA":
                        #region RNA
                        deck = "Tempered Steel DOM";
                        AddDeck(0);
                        #endregion
                        break;
                    case "GRN":
                        #region GRN
                        deck = "Tempered Steel DOM";
                        AddDeck(0);
                        #endregion
                        break;
                    case "M19":
                        #region M19
                        deck = "Tempered Steel DOM";
                        AddDeck(6);
                        #endregion
                        break;
                    case "DOM":
                        #region DOM
                        deck = "Tempered Steel DOM";
                        AddDeck(24);
                        #endregion
                        break;
                    case "RIX":
                        #region RIX
                        deck = "Tempered Steel DOM";
                        AddDeck(0);
                        #endregion
                        break;
                    case "XLN":
                        #region XLN
                        deck = "Tempered Steel DOM";
                        AddDeck(0);
                        #endregion
                        break;
                    case "AKR":
                        #region AKR
                        deck = "Tempered Steel DOM";
                        AddDeck(0);
                        #endregion
                        break;
                    default:
                        break;
                }
                #endregion

                // Get if you can craft the last deck in the above information
                Console.WriteLine("Craftable? " + GetIfCraftable());

                // Get collection results
                GetCollectionResults();
            }
            else
            {
                // Calculate cheapest decks
                CalculateCheapestDecks();
            }
            Console.ReadKey();
        }

        // SUMMARY: Get a deck and how many cards are needed for each rarity and duplicate amount. Then, with the packs given to us, add this to a dictionary
        // This part asks for really specific values (how many cards different cards? How many duplicates each?) which resulted in this somewhat confusing information method.
        // However, using this method makes the calculator very accurate, as with this specific information, it can take things like crafted cards into account.
        static void AddDeck(int packs)
        {
            // Setup
            #region Initialize
            // This is to make sure there will be no error, as we can't return null
            cardsToOpen = new int[,]
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            };

            // The same as cardsToOpen, but these have to be crafted
            cardsForcedToCraft = new int[,]
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            };
            #endregion

            // Template
            #region Templates
            #region Standard Template
            #region Deck
            if (deck.Contains("Template"))
            {
                if (set == "ZNR")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "M21")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "IKO")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "THB")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "ELD")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
            }
            #endregion

            #region Sideboard
            if (deck.Contains("Template sideboard"))
            {
                if (set == "ZNR")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "M21")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "IKO")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "THB")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "ELD")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
            }
            #endregion

            #region Deck
            if (deck.Contains("Template"))
            {
                if (!deck.Contains("sideboard"))
                {

                }
                else
                {

                }
            }
            #endregion
            #endregion
            #endregion

            // Decks for standard
            #region Standard
            #region WUBRG Plants
            #region WUBRG Plants
            if (deck.Contains("WUBRG Plants"))
            {
                if (set == "ZNR")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 3, 3, 3, 3 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "M21")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 2, 2, 2, 2 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "IKO")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "THB")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "ELD")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 1, 1, 0, 0 }
                    };
                }
            }
            #endregion

            #region Sideboard
            if (deck.Contains("Template sideboard"))
            {
                if (set == "ZNR")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "M21")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "IKO")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "THB")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "ELD")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
            }
            #endregion

            #region Deck
            if (deck.Contains("WUBRG Plants"))
            {
                if (!deck.Contains("sideboard"))
                {
                    if (deck.Contains("1" + set))
                    {
                        // 4 commons
                        cardsToOpen[0, 0]++;
                        cardsToOpen[0, 1]++;
                        cardsToOpen[0, 2]++;
                        cardsToOpen[0, 3]++;
                    }
                    if (deck.Contains("2" + set))
                    {
                        // 4 commons
                        cardsForcedToCraft[0, 0]++;
                        cardsForcedToCraft[0, 1]++;
                        cardsForcedToCraft[0, 2]++;
                        cardsForcedToCraft[0, 2]++;
                    }
                    if (deck.Contains("3" + set))
                    {
                        // 4 Rares
                        cardsToOpen[2, 0]++;
                        cardsToOpen[2, 1]++;
                        cardsToOpen[2, 2]++;
                        cardsToOpen[2, 3]++;
                    }
                }
                else
                {

                }
            }
            #endregion
            #endregion

            #region Selesnya Counters
            #region Selesnya Counters
            if (deck.Contains("Selesnya Counters"))
            {
                if (set == "ZNR")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 2, 2, 0, 0 },
                        { 4, 4, 4, 4 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "M21")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 3, 3, 3, 3 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "IKO")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 1, 1, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "THB")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1, 1, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "ELD")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 1 - 1, 1 - 1, 1 - 1, 1 - 1 },
                        { 0, 0, 0, 0 }
                    };
                }
            }
            #endregion

            #region Sideboard
            if (deck.Contains("Template sideboard"))
            {
                if (set == "ZNR")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "M21")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "IKO")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "THB")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "ELD")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
            }
            #endregion

            #region Deck
            if (deck.Contains("Template"))
            {
                if (!deck.Contains("sideboard"))
                {

                }
                else
                {

                }
            }
            #endregion
            #endregion

            #region Rakdos Party Aggro
            #region Rakdos Party Aggro
            if (deck.Contains("Rakdos Party Aggro") && !deck.Contains("sideboard"))
            {
                if (set == "ZNR")
                {
                    cardsToOpen = new int[,]
                    {
                        { 5, 5, 3, 3 },
                        { 2, 2, 2, 1 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "M21")
                {
                    cardsToOpen = new int[,]
                    {
                        { 1, 1, 1, 1 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "IKO")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 1, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "THB")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 1, 0, 0, 0 }
                    };
                }
                if (set == "ELD")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 1, 0, 0, 0 },
                        { 1, 0, 0, 0 }
                    };
                }
            }
            #endregion

            #region Duplicates
            if (deck.Contains("Boros Warriors"))
            {
                if (deck.Contains(set))
                {
                    // 4 commons
                    cardsForcedToCraft[0, 0]++;
                    cardsForcedToCraft[0, 1]++;
                    cardsForcedToCraft[0, 2]++;
                    cardsForcedToCraft[0, 2]++;
                }
            }
            #endregion
            #endregion

            #region Boros Warriors
            #region Boros Warriors
            if (deck.Contains("Boros Warriors") && !deck.Contains("sideboard"))
            {
                if (set == "ZNR")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 6 - 1, 5 - 1, 4 - 1, 4 },
                        { 5, 5, 4, 4 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "M21")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "IKO")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 }
                    };
                }
                if (set == "THB")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 2, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "ELD")
                {
                    cardsToOpen = new int[,]
                     {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 1, 0, 0, 0 }
                     };

                }
            }
            #endregion

            #region Duplicates
            if (deck.Contains("Boros Warriors"))
            {
                if (deck.Contains("1" + set))
                {
                    // 4 rares
                    cardsToOpen[2, 0]++;
                    cardsToOpen[2, 1]++;
                    cardsToOpen[2, 2]++;
                    cardsToOpen[2, 2]++;
                }
                if (deck.Contains("2" + set))
                {
                    // 2 commons
                    cardsForcedToCraft[0, 0]++;
                    cardsForcedToCraft[0, 1]++;
                }
            }
            #endregion
            #endregion

            #region Simic Scute
            #region Simic Scute
            if (deck.Contains("Simic Scute") && !deck.Contains("sideboard"))
            {
                if (set == "ZNR")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "IKO")
                {
                    cardsToOpen = new int[,]
                    {
                        { 1, 1, 1, 1 },
                        { 4, 4, 4, 4 },
                        { 1, 1, 0, 0 },
                        { 1, 1, 0, 0 }
                    };
                }
                if (set == "ELD")
                {
                    cardsToOpen = new int[,]
                     {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 }
                     };

                }
            }
            #endregion

            #region Simic Scute sideboard
            if (deck.Contains("Simic Scute sideboard"))
            {
                if (set == "ZNR")
                {
                    cardsToOpen = new int[,]
                    {
                        { 2 - 2, 2 - 2, 1 - 1, 0 },
                        { 0, 0, 0, 0 },
                        { 1, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "M21")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "ELD")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
            }
            #endregion

            #region Duplicates
            if (deck.Contains("Simic Scute"))
            {
                if (!deck.Contains("sideboard"))
                {
                    if (deck.Contains("1" + set))
                    {
                        // 4 rares
                        cardsToOpen[2, 0]++;
                        cardsToOpen[2, 1]++;
                        cardsToOpen[2, 2]++;
                        cardsToOpen[2, 2]++;
                    }
                    if (deck.Contains("2" + set))
                    {
                        // 4 commons
                        cardsForcedToCraft[0, 0]++;
                        cardsForcedToCraft[0, 1]++;
                        //cardsForcedToCraft[0, 2]++;
                        //cardsForcedToCraft[0, 2]++;
                    }
                }
                else
                {
                    if (deck.Contains(set))
                    {
                        // 2 commons
                        //cardsToOpen[0, 0]++;
                        //cardsToOpen[0, 1]++;
                    }
                }
            }
            #endregion
            #endregion

            #region Simic Kicker
            #region Simic Kicker
            if (deck.Contains("Simic Kicker") && !deck.Contains("sideboard"))
            {
                if (set == "ZNR")
                {
                    cardsToOpen = new int[,]
                    {
                        { 3, 3, 3, 2 },
                        { 4, 4, 4, 4 },
                        { 4, 4, 4, 3 },
                        { 0, 0, 0, 0 }
                    };
                }
            }
            #endregion

            #region Simic Kicker sideboard
            if (deck.Contains("Simic Kicker sideboard"))
            {
                if (set == "ZNR")
                {
                    cardsToOpen = new int[,]
                    {
                        { 1, 1, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "THB")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "ELD")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1, 1, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
            }
            #endregion

            #region Duplicates
            if (deck.Contains("Simic Kicker"))
            {
                if (!deck.Contains("sideboard"))
                {
                    if (deck.Contains("1" + set))
                    {
                        // 4 rares
                        cardsToOpen[2, 0]++;
                        cardsToOpen[2, 1]++;
                        cardsToOpen[2, 2]++;
                        cardsToOpen[2, 2]++;
                    }
                    if (deck.Contains("2" + set))
                    {
                        // 2 commons
                        cardsForcedToCraft[0, 0]++;
                        cardsForcedToCraft[0, 1]++;
                    }
                }
                else
                {
                    if (deck.Contains(set))
                    {
                        // 3 commons
                        cardsToOpen[0, 0]++;
                        cardsToOpen[0, 1]++;
                        cardsToOpen[0, 2]++;
                    }
                }
            }
            #endregion
            #endregion
            #endregion

            // Decks for Historic
            #region Historic
            #region Tempered Steel
            if (deck.Contains("Tempered Steel") && !deck.Contains("sideboard"))
            {
                if (set == "ELD")
                {
                    cardsToOpen = new int[,]
                    {
                        { 2, 2, 2, 2 },
                        { 2, 2, 2, 2 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "M20")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "M19")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1, 1, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };

                }
                if (set == "DOM")
                {
                    cardsToOpen = new int[,]
                    {
                        { 2, 2, 1, 1 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
            }
            #endregion

            #region Tempered Steel sideboard
            if (deck.Contains("Tempered Steel sideboard"))
            {
                if (set == "THB")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1 - 1, 1 - 1, 1 - 1, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "ELD")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "GRN")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "M19")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 1, 1 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (set == "DOM")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1, 1, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
            }
            #endregion

            #region Duplicates
            if (deck.Contains("Tempered Steel") && !deck.Contains("sideboard"))
            {
                if (deck.Contains("1" + set))
                {
                    cardsForcedToCraft = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (deck.Contains("2" + set))
                {
                    cardsForcedToCraft = new int[,]
                    {
                        { 1, 1, 1, 1 },
                        { 1, 1, 1, 1 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 }
                    };
                }
                if (deck.Contains("2") && set == "ELD")
                {
                    cardsToOpen[0, 0]--;
                    cardsToOpen[0, 1]--;
                    cardsToOpen[0, 2]--;
                    cardsToOpen[0, 3]--;
                }
            }
            #endregion
            #endregion

            // How many packs need to be opened to get all cards of the rarity
            // The * means that it is only an average amount as these rarities do not have the duplicate protection rule
            #region All rarities
            // 98* packs
            // RIX 68* packs
            // AKR 104* packs
            #region All Commons
            #region ZNR
            if (deck == "All Commons ZNR")
            {
                if (set == "ZNR")
                {
                    // DO NOT CHANGE THIS IS RIGHT
                    cardsToOpen = new int[,]
                    {
                        { 101, 101, 101, 101 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    };
                }
            }
            #endregion
            #endregion

            // 158* packs
            #region All Uncommon
            if (deck == "All Uncommon")
            {
                cardsToOpen = new int[,]
                {
                        { 0, 0, 0, 0 },
                        { 80, 80, 80, 80},
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                };
            }
            #endregion

            // 215 packs
            // ZNR 260
            #region All Rares
            if (deck == "All Rares")
            {
                if (set == "ZNR")
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 64 - 10, 64 - 10, 64 - 9, 64 - 8 },
                        { 0, 0, 0, 0 }
                    };
                }
                else
                {
                    cardsToOpen = new int[,]
                    {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 53, 53, 53, 53 },
                        { 0, 0, 0, 0 }
                    };
                }
            }
            #endregion

            // 321 packs
            #region All Mythic
            if (deck == "All Mythics")
            {
                cardsToOpen = new int[,]
                {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 15, 15, 15, 15 }
                };
            }
            #endregion
            #endregion

            // Add information
            packsForDecks.Add(new PacksForDecksDictionary(packs, (int[,])cardsToOpen.Clone(), (int[,])cardsForcedToCraft.Clone()));
        }

        #region Collection management
        // Get how many cards we've gotten from our packs
        static void GetCollectionResults()
        {
            // Declare. This variable is the total amount of cards we've opened per rarity
            double totalCardsPerRarity;

            // Go through rarities
            for (int i = 0; i < cardCopiesOwned.GetLength(0); i++)
            {
                // Total opened cards per rarity
                totalCardsPerRarity = 0;

                // Go through amount of duplicates
                for (int j = 0; j < cardCopiesOwned.GetLength(1); j++)
                {
                    // Add to total and print "Rarity/Cards gotten/Cards in set"
                    totalCardsPerRarity += cardCopiesOwned[i, j];
                    Console.WriteLine("Of the rarity " + rarityNames[i] + ", you have opened " + (j + 1) + " duplicates a total of " + cardCopiesOwned[i, j] + " times out of " + cardsInSet[i] + " cards.");
                }
                // Print "Total cards opened for rarity/Opened wildcards"
                Console.WriteLine("TOTAL: " + totalCardsPerRarity);
                Console.WriteLine("Wild cards: " + openedWildcards[i]);

                Console.WriteLine();
            }

            // Print vault points
            Console.WriteLine("Vault Points: " + vaultPoints);
        }

        static void InitializeVariables()
        {
            #region Initialize
            // MTG amount of rarities and duplicate limit
            maxDuplicates = 4;
            rarityAmount = 4;

            // Wildcard chances per pack (rather than filling 20% or 1/5 we fill in 5 to make it easier later)
            randomWildcardRouletTimer = new int[] { 3, 5, 30, 30 };

            // Guaranteed wildcards after specific amount of packs (not chance, always every time these numbers are reached. Commons [0] aren't given away this method)
            wildcardRouletTimer = new int[] { 0, 6, 6, 30 };

            // Cards in a pack reset
            cardsInPacks = new double[] { 5, 2, 0.875f, 0.125f };

            if (set == "ZNR")
            {
                cardsInPacks = new double[] { 5, 2, 6.4f / 7.4f, 1f / 7.4f };
            }
            if (set == "AKR")
            {
                cardsInPacks = new double[] { 5, 2, 0.8f, 0.2f };
            }

            // Opened wildcards
            openedWildcards = new double[4];

            // Cards opened
            cardCopiesOwned = new double[rarityAmount, maxDuplicates];

            // This variable is used to calculate the cheapest deck.
            packsAlreadyOpened = 0;

            // Vualt points
            vaultPoints = 0;

            // Set the values for how many cards are in the set
            cardsInSet = new int[] { 101, 80, 53, 15 };

            if (set == "ZNR")
            {
                cardsInSet = new int[] { 101, 80, 64, 20 };
            }
            if (set == "RIX")
            {
                cardsInSet = new int[] { 70, 60, 48, 13 };
            }
            if (set == "XLN")
            {
                cardsInSet = new int[] { 101, 80, 63, 15 };
            }
            if (set == "AKR")
            {
                cardsInSet = new int[] { 108, 90, 74, 31 };
            }
            #endregion
        }

        static void AddCardsToCollection(int packsGoingToOpen, bool restart)
        {
            // Reset variables if we start a new collection
            if (restart)
            {
                InitializeVariables();
            }

            // PacksGoingToOpen is at the current moment a value which is the total packs opened. By substract we get a value what the name sugests
            // (Because of the way the code is made, it's best to do it this way)
            // (It is also needed to make this a different variable from packsToOpen because that variable constantly changes in the cheapest deck calculation and substracting turns this possibly into an infinite loop)
            packsGoingToOpen -= packsAlreadyOpened;

            // Cards in a pack reset
            cardsInPacks = new double[] { 5, 2, 0.875f, 0.125f };

            if (set == "ZNR")
            {
                cardsInPacks = new double[] { 5, 2, 6.4f / 7.4f, 1f / 7.4f };
            }
            if (set == "AKR")
            {
                cardsInPacks = new double[] { 5, 2, 0.8f, 0.2f };
            }

            // Multiply the cards in the pack with the packs we are going to open.
            for (int i = 0; i < cardsInPacks.Length; i++)
            {
                cardsInPacks[i] *= packsGoingToOpen;
            }

            // Go through rarities
            for (int i = 0; i < openedWildcards.Length; i++)
            {
                // Get opened wildcards from the packs we are going to open
                openedWildcards[i] = packsGoingToOpen / (double)randomWildcardRouletTimer[i];

                // Then, depending on rarity, substract the cards in the pack (because wildcards replace a card in a pack)
                if (i <= 1)
                {
                    cardsInPacks[i] -= openedWildcards[i];
                }
                else
                {
                    // With rare and mythic rare, this works a little different. BOTH have a 1 in 30 chance to be replaced with a wildcard, but share the same card slot.
                    // Since both share the same slot, deviding by 30 will result in more rare wildcards as the calculator will open more of these and thus devide a higher 
                    // number than the mythic rares
                    // To solve this, we must multiply the cards we've opened to give both an equal chance and devide that number.
                    // The numbers, not looking at scale, are ( (chance on getting rarity card - chance on getting rarity WILDcard) / total chance of both )
                    // To prove that this works, you can start the program, type any set and open 1 pack. Both rarities have 0.033 wildcard, 1/30 and thus get an equal amount
                    if (set == "ZNR")
                    {
                        if (i == 2)
                        {
                            cardsInPacks[i] = (packsGoingToOpen) * 179.2 / 222.0;
                        }
                        else
                        {
                            cardsInPacks[i] = (packsGoingToOpen) * 28.0 / 222.0;
                        }
                    }
                    else if (set == "AKR")
                    {
                        if (i == 2)
                        {
                            cardsInPacks[i] = (packsGoingToOpen) * 112.0 / 150.0;
                        }
                        else
                        {
                            cardsInPacks[i] = (packsGoingToOpen) * 28.0 / 150.0;
                        }
                    }
                    else
                    {
                        if (i == 2)
                        {
                            cardsInPacks[i] = (packsGoingToOpen) * 196.0 / 240.0;
                        }
                        else
                        {
                            cardsInPacks[i] = (packsGoingToOpen) * 28.0 / 240.0;
                        }
                    }
                }

                // Then get the remaining wildcards
                openedWildcards[i] += packsAlreadyOpened / (double)randomWildcardRouletTimer[i];

                // Then get the wildcards from the roulet timer
                if (wildcardRouletTimer[i] != 0)
                {
                    openedWildcards[i] += (packsGoingToOpen + packsAlreadyOpened) / wildcardRouletTimer[i];
                }
            }

            // And substract some rares depending on how many mythics we opened with the roulet
            openedWildcards[2] -= (packsGoingToOpen + packsAlreadyOpened) / wildcardRouletTimer[3];

            // ADD CARDS TO THE COLLECTION
            for (int i = 0; i < cardsInPacks.Length; i++)
            {
                for (int j = 1; j <= cardsInPacks[i]; j++)
                {
                    AddCard(i, 1);
                }
                AddCard(i, cardsInPacks[i] % 1);
            }

            // Get vaultpoints and add some wildcards
            openedWildcards[1] += vaultPoints / 1000 * 3;
            openedWildcards[2] += vaultPoints / 1000 * 2;
            openedWildcards[3] += vaultPoints / 1000;

            // Set packsAlreadyOpened to the right value
            packsAlreadyOpened += packsGoingToOpen;
        }
        #endregion

        #region Calculation management
        static bool GetIfCraftable()
        {
            // Setup
            double[] wildcardUsed = new double[4];
            double[,] specificWildcardUsed = new double[4, 4];
            double wildcardsJustUsed;

            // Go through all the premade decks
            for (int a = 0; a < packsForDecks.Count; a++)
            {
                #region Not the last deck
                // Checks if we are at the last deck or not
                if (a < packsForDecks.Count - 1)
                {
                    // Add cards to the collection or start it if we're at the first deck
                    AddCardsToCollection(packsForDecks[a].Key, a == 0);

                    // Go through rarities
                    for (int i = 0; i < cardCopiesOwned.GetLength(0); i++)
                    {
                        // Go through duplicates
                        for (int j = 0; j < cardCopiesOwned.GetLength(1); j++)
                        {
                            // Forced cards to craft
                            int forcedToCraft = 0;

                            // Get data
                            forcedToCraft = packsForDecks[a].ForcedValue[i, j];

                            // This value is the amount of wildcards we're going to use. 
                            // It gets the percentage of the set we don't have. This is our chances we didn't open a card we wanted and we multiply this with the amount of cards we wanted to get.
                            // This is the value we didn't open and needed wildcards to use on.
                            wildcardsJustUsed = (1 - cardCopiesOwned[i, j] / cardsInSet[i]) * packsForDecks[a].Value[i, j];

                            // Wildcards used per rarity. ForcedToCraft is only used here as forcedToCraft cards aren't in the cardCopiesOwned and
                            // specificWildcardUsed interacts with that variable. These cards will typicaly be basic land cards which you can't open in MTG Arena packs
                            wildcardUsed[i] += wildcardsJustUsed + forcedToCraft;

                            // Wildcards used per rarity and duplicate amount.
                            specificWildcardUsed[i, j] += wildcardsJustUsed;

                            // Add this wildcard to the collection (since we crafted a card using it).
                            cardCopiesOwned[i, j] += wildcardsJustUsed;
                        }
                    }
                }
                #endregion

                #region Last deck
                else
                {
                    // Add cards to the collection or start it if we're at the first deck (first variable is different from the previous function)
                    AddCardsToCollection(packsToOpen, a == 0);

                    // Go through rarities
                    for (int i = 0; i < cardCopiesOwned.GetLength(0); i++)
                    {
                        // Reset craft goal and progress
                        craftGoal = 0;
                        craftProgress = 0;

                        // Go through duplicates
                        for (int j = 0; j < cardCopiesOwned.GetLength(1); j++)
                        {
                            // Add all cards needed for the deck (with the respected rarity) to the craft goal.
                            int ForcedToCraft = 0;

                            ForcedToCraft = packsForDecks[a].ForcedValue[i, j];

                            craftGoal += cardsToOpen[i, j] + ForcedToCraft;

                            // Calculate the craft progress which is the percentage of the amount of cards we opened out of the set that aren't used wildcards (as those are removed hard coded/manually)
                            // This amount is then multiplied by the amount of cards we want to open
                            craftProgress += (cardCopiesOwned[i, j] - specificWildcardUsed[i, j]) / (cardsInSet[i] - specificWildcardUsed[i, j]) * cardsToOpen[i, j];
                        }

                        // Add the unused wildcards to the progress
                        craftProgress += openedWildcards[i] - wildcardUsed[i];

                        // If the progress is lower than the goal, we can't craft the deck
                        if (craftProgress < craftGoal)
                        {
                            return false;
                        }
                    }

                }
                #endregion
            }
            // If we reached the end, the deck is craftable
            return true;
        }

        static void CalculateCheapestDecks()
        {
            // Setup. The difference between the two following dictionaries is that packsPerSetsy remembers for EACH SET how many packs were opened for AN ENTIRE DECK 

            // while packsForDecks is a bit more complicated. This dictionary only remembers how many cards for A SINGLE SET it needs to open (Value) and with
            // how many packs this should be TESTED with.

            // PacksForSets is meant to remember info while PacksForDecks is meant to be tested with (in the case of ordering decks from cheapest to most
            // expensive)
            packsPerSets = new List<PacksForSetsDictionary>();
            packsForDecks = new List<PacksForDecksDictionary>();

            // packsOpenedForCurrentDeck remembers the total pack cost of a deck, not per set.
            // packsOpenedForAllDecks remembers the packs opened per set.
            int[] packsOpenedForCurrentDeck;
            int[,] packsOpenedForAllDecks;

            // As the name says
            int cheapestAmountOfPacks;
            int cheapestIndex;

            // Used when we know the cheapest deck and to feed info to packsPerSets.
            int[] packsOpenedForCheapestDeck;

            // Used to know if there is a tie
            bool danger;

            // Run the code until all the premade decks are categorized
            while (deckMemory.Count > 0)
            {
                // Setup
                packsOpenedForCurrentDeck = new int[deckMemory.Count];
                packsOpenedForAllDecks = new int[deckMemory.Count, setMemory.Length];
                packsOpenedForCheapestDeck = new int[setMemory.Length];
                danger = false;

                // Let's hope we never have to open 4095 packs
                cheapestAmountOfPacks = 4095;

                // -1 since we can't have a negative array value
                cheapestIndex = -1;

                // Go through the premade decks we've made
                for (int i = 0; i < deckMemory.Count; i++)
                {
                    // Go through the sets
                    for (int j = 0; j < setMemory.Length; j++)
                    {
                        // Set set variable and reset other variables
                        set = setMemory[j];
                        InitializeVariables();

                        // Reset deck list we've already crafted
                        packsForDecks = new List<PacksForDecksDictionary>();

                        // Add the decks to the packsForDeckList
                        for (int k = 0; k < packsPerSets.Count; k++)
                        {
                            deck = packsPerSets[k].Key;

                            AddDeck(packsPerSets[k].Value[j]);
                        }

                        // Get deck
                        deck = deckMemory[i];

                        // Zero because it doesn't mater what value we give it.
                        AddDeck(0);

                        // Give packs to open the right value
                        if (packsPerSets.Count == 0)
                        {
                            packsToOpen = 0;
                        }
                        else
                        {
                            packsToOpen = packsPerSets[packsPerSets.Count - 1].Value[j];
                        }

                        // Open packs until the deck is craftable
                        while (!GetIfCraftable())
                        {
                            packsToOpen++;
                        }

                        // Add cards to the memory
                        packsOpenedForCurrentDeck[i] += packsToOpen;
                        packsOpenedForAllDecks[i, j] = packsToOpen;
                    }

                    if (cheapestAmountOfPacks == packsOpenedForCurrentDeck[i])
                    {
                        danger = true;
                    }

                    // If the deck is cheaper than the current deck, make it the cheapest
                    if (cheapestAmountOfPacks > packsOpenedForCurrentDeck[i])
                    {
                        cheapestAmountOfPacks = packsOpenedForCurrentDeck[i];
                        cheapestIndex = i;

                        danger = false;
                    }
                }
                // If there is a tie between decks, inform us
                if (danger)
                {
                    Console.WriteLine("WARNING TIE WARNING TIE WARNING TIE WARNING TIE WARNING TIE WARNING TIE WARNING TIE WARNING TIE WARNING TIE WARNING TIE");

                    // Then show the decks that are tied with their pack values
                    for (int i = 0; i < packsOpenedForCurrentDeck.Length; i++)
                    {
                        if (packsOpenedForCurrentDeck[i] == cheapestAmountOfPacks)
                        {
                            Console.WriteLine("DECK: " + deckMemory[i] + " - PACKS: " + packsOpenedForCurrentDeck[i]);
                        }

                    }
                }
                // Print name of cheapest deck
                Console.WriteLine("The cheapest deck is " + deckMemory[cheapestIndex] + ". Currently opened " + packsOpenedForCurrentDeck[cheapestIndex]);

                // Add decks that mid calculation would turn cheaper than pre calculated decks (HARD CODED)
                if (deckMemory[cheapestIndex].Contains("Simic Kicker") && !deck.Contains("sideboard"))
                {
                    //deckMemory.Add("Simic Scute 2M21");
                    //deckMemory.Add("Simic Scute 2IKO");
                    deckMemory.Add("Simic Scute 2ELD");
                    //deckMemory.Add("Simic Scute 2M20");
                }
                if (deckMemory[cheapestIndex].Contains("Simic Scute") && !deck.Contains("sideboard"))
                {
                    //deckMemory.Add("All Commons ZNR");
                }
                //if (deckMemory[cheapestIndex].Contains("Boros Warriors 1M21 2IKO"))
                //{
                //    deckMemory.Add("Tempered Steel sideboard");

                //    deckMemory.Add("Simic Kicker sideboard ZNR");
                //    //deckMemory.Add("Simic Kicker sideboard M20");
                //    //deckMemory.Add("Simic Kicker sideboard RIX");

                //    deckMemory.Add("Simic Scute sideboard ZNR");
                //    //deckMemory.Add("Simic Scute sideboard M20");
                //    //deckMemory.Add("Simic Scute sideboard RIX");
                //}

                // Get all packs opened for the specific sets of the cheapest deck and print "Set/Packs opened"
                for (int i = 0; i < setMemory.Length; i++)
                {
                    Console.WriteLine("This many packs where opened for " + setMemory[i] + ": " + packsOpenedForAllDecks[cheapestIndex, i]);

                    packsOpenedForCheapestDeck[i] = packsOpenedForAllDecks[cheapestIndex, i];
                }

                // Add the result to packsPerSets and remove the craftable deck from the list as if we crafted it.
                packsPerSets.Add(new PacksForSetsDictionary(deckMemory[cheapestIndex], (int[])packsOpenedForCheapestDeck.Clone()));
                deckMemory.Remove(deckMemory[cheapestIndex]);

                Console.WriteLine();
            }
        }
        #endregion

        #region Pack management
        // Add cards to the collection
        static void AddCard(int rarityIndex, double card)
        {
            // CardPercentage is a card we've opened and want to insert in our collection. PreviousPercentage is a memory variable
            double cardPercentage = card;
            double previousPercentage;

            // In case of rare and mythic rare, add duplicate protection (see that function)
            if (cardPercentage > 0 && (rarityIndex == 2 || rarityIndex == 3))
            {
                AddFullCard(rarityIndex, cardPercentage);
            }
            else
            {
                for (int i = 0; i < maxDuplicates; i++)
                {
                    // 1 First, turn previousPercentage in the current one.
                    // 2 Then, for the current duplicate amount, get what the chances are to open a card we haven't had with this many duplicates
                    // 3 Devide this number with the total amount of cards in this rarity so we get a value between 1 and 0 (or thus a percentage).
                    // 4 Multiply this with the card we have *left over* and add the percentage.
                    previousPercentage = cardPercentage;
                    cardPercentage = ((cardsInSet[rarityIndex] - cardCopiesOwned[rarityIndex, i]) / cardsInSet[rarityIndex]) * cardPercentage;
                    cardCopiesOwned[rarityIndex, i] += cardPercentage;

                    // 5 The cardpercentage now becomes the chance we didn't open on this duplicate amount. The *left over*
                    cardPercentage = previousPercentage - cardPercentage;
                }

                // Simple vaultpoints addition. 5th duplications are turned into vaultpoints
                if (rarityIndex == 0)
                {
                    vaultPoints += cardPercentage;
                }
                else
                {
                    vaultPoints += cardPercentage * 3;
                }
            }
        }

        static void AddFullCard(int rarityIndex, double card)
        {
            double cardPercentage = card;
            double previousPercentage;

            for (int i = 0; i < maxDuplicates; i++)
            {
                // Pretty much the same except the numbers are decreased with the 4th duplicate amount of cards we've opened.
                // This is because now, the calculation works with higher chance numbers, BUT the 4th duplication will ALWAYS be a 100% as the substraction becomes 0
                previousPercentage = cardPercentage;
                cardPercentage = (((cardsInSet[rarityIndex] - cardCopiesOwned[rarityIndex, 3]) - (cardCopiesOwned[rarityIndex, i] - cardCopiesOwned[rarityIndex, 3])) / (cardsInSet[rarityIndex] - cardCopiesOwned[rarityIndex, 3])) * cardPercentage;
                cardCopiesOwned[rarityIndex, i] += cardPercentage;

                cardPercentage = previousPercentage - cardPercentage;
            }
        }
        #endregion
    }
}

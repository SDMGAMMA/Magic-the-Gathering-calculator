using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTG_calculator
{
    static class Calculations
    {
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
                            // This value is the amount of wildcards we're going to use. 
                            // It gets the percentage of the set we don't have. This is our chances we didn't open a card we wanted and we multiply this with the amount of cards we wanted to get plus the basic lands which we can't get from packs.
                            // This is the value we didn't open and needed wildcards to use on.
                            int forcedToCraft = 0;

                            forcedToCraft = packsForDecks[a].ForcedValue[i, j];

                            wildcardsJustUsed = (1 - cardCopiesOwned[i, j] / cardsInSet[i]) * packsForDecks[a].Value[i, j];

                            // Wildcards used per rarity.
                            wildcardUsed[i] += wildcardsJustUsed;

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
            // Setup. The difference between the two is that packsPerSet remembers for each set how many packs were opened 
            // while packsForDecks remembers the cards we want and only accepts packs for a specific set, not all
            packsPerSets = new List<PacksForSetsDictionary>();
            packsForDecks = new List<PacksForDecksDictionary>();

            int[] packsOpenedForCurrentDeck;
            int[,] packsOpenedForAllDecks;

            int cheapestAmountOfPacks;
            int cheapestIndex;

            int[] packsOpenedForCheapestDeck;

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
                if (danger)
                {
                    Console.WriteLine("WARNING TIE WARNING TIE WARNING TIE WARNING TIE WARNING TIE WARNING TIE WARNING TIE WARNING TIE WARNING TIE WARNING TIE");

                    for (int i = 0; i < packsOpenedForCurrentDeck.Length; i++)
                    {
                        if (packsOpenedForCurrentDeck[i] == cheapestAmountOfPacks)
                        {
                            Console.WriteLine("DECK: " + deckMemory[i] + " - PACKS: " + packsOpenedForCurrentDeck[i]);
                        }

                    }
                }
                // Print name of cheapest deck
                Console.WriteLine("The cheapest deck is " + deckMemory[cheapestIndex]);

                // Add decks that mid calculation would turn cheaper than pre calculated decks (HARD CODED)
                if (deckMemory[cheapestIndex] == "Template 1")
                {
                    deckMemory.Add("Template 2");
                }

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
    }
}

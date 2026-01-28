using Airk.World;

namespace Airk.State;

public static class WorldBuilder
{
    public static GameState CreateNewGame()
    {
        var state = new GameState();

        state.Rooms["alley"] = new Room
        {
            Id = "alley",
            Name = "Dark Alley",
            Description = "Rain drips from rusted fire escapes above. Neon signs flicker through the smog, casting red and blue shadows on the wet pavement. A dumpster overflows with discarded tech. To the north, a heavy steel door leads into a building. The alley continues east toward the main street.",
            Exits = new Dictionary<string, string>
            {
                ["north"] = "bar",
                ["east"] = "street"
            },
            Items = new List<string> { "credstick", "datapad" }
        };

        state.Rooms["bar"] = new Room
        {
            Id = "bar",
            Name = "The Rusty Circuit",
            Description = "Synth music pulses from cracked speakers. A few figures hunch over drinks at the bar, their faces illuminated by the glow of their neural interfaces. The bartender, a chrome-armed woman, polishes a glass with mechanical precision. The exit south leads back to the alley.",
            Exits = new Dictionary<string, string>
            {
                ["south"] = "alley"
            },
            Items = new List<string>()
        };

        state.Rooms["street"] = new Room
        {
            Id = "street",
            Name = "Neon Boulevard",
            Description = "Holographic advertisements tower above, selling everything from neural upgrades to synthetic meals. Drones buzz overhead, their cameras scanning the crowds. A vendor cart sells hot noodles, steam rising into the polluted air. The alley is to the west, and a metro station entrance is to the north.",
            Exits = new Dictionary<string, string>
            {
                ["west"] = "alley",
                ["north"] = "metro"
            },
            Items = new List<string>()
        };

        state.Rooms["metro"] = new Room
        {
            Id = "metro",
            Name = "Metro Station Entrance",
            Description = "Turnstiles block access to the underground. A bored security guard watches from behind bulletproof glass. A sign reads 'Fare: 10 credits'. The boulevard is to the south.",
            Exits = new Dictionary<string, string>
            {
                ["south"] = "street"
            },
            Items = new List<string> { "transit-map" }
        };

        // NPCs
        state.Npcs["chrome"] = new Npc
        {
            Id = "chrome",
            Name = "Chrome",
            Description = "A woman with chrome cybernetic arms. Her eyes have a faint blue glow - optic implants. She moves with practiced efficiency.",
            RoomId = "bar",
            Dialogue = new List<DialogueLine>
            {
                new DialogueLine
                {
                    Id = "chrome_intro",
                    Text = "You look like you've had a rough night. I'm Chrome. You need something?",
                    SetsFlag = "met_chrome",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "chrome_job_offer",
                    RequiresFlag = "met_chrome",
                    Text = "Look, I could use someone to run a package to the metro station. 10 credits if you're interested. Just say 'talk' again when you're ready.",
                    SetsFlag = "job_offered",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "chrome_job_accept",
                    RequiresFlag = "job_offered",
                    Text = "Good. Take this package to the guard at the metro. Don't open it. Come back when it's done.",
                    SetsFlag = "job_accepted",
                    GivesItem = "package",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "chrome_job_done",
                    RequiresFlag = "job_complete",
                    Text = "Nice work. Here's your 10 credits. Maybe I'll have more work for you later.",
                    GivesCredits = 10,
                    SetsFlag = "job_paid",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "chrome_waiting",
                    RequiresFlag = "job_accepted",
                    Text = "The metro station is north from Neon Boulevard. Get moving.",
                    Repeatable = true
                },
                new DialogueLine
                {
                    Id = "chrome_idle",
                    RequiresFlag = "job_paid",
                    Text = "Nothing right now. Check back later.",
                    Repeatable = true
                }
            }
        };

        state.Npcs["guard"] = new Npc
        {
            Id = "guard",
            Name = "Security Guard",
            Description = "A bored-looking guard in corporate security armor. He watches the turnstiles with glazed eyes.",
            RoomId = "metro",
            Dialogue = new List<DialogueLine>
            {
                new DialogueLine
                {
                    Id = "guard_package",
                    RequiresFlag = "job_accepted",
                    RequiresItem = "package",
                    Text = "Package from Chrome? About time. Tell her we're square now.",
                    SetsFlag = "job_complete",
                    RemovesItem = "package",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "guard_no_money",
                    Text = "Fare's 10 credits. No exceptions.",
                    Repeatable = true
                }
            }
        };

        state.LastMessage = "You wake up in a dark alley. Your head pounds. You don't remember how you got here. A flickering neon sign above reads 'Welcome to Kreznik' - but that doesn't help. Everyone knows Kreznik.";

        return state;
    }
}

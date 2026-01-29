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
            ShortDescription = "A narrow alley between crumbling buildings, lit by flickering neon.",
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
            ShortDescription = "A dimly lit synth bar behind a heavy steel door.",
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
            ShortDescription = "A wide boulevard crowded with holographic ads and buzzing drones.",
            Description = "Holographic advertisements tower above, selling everything from neural upgrades to synthetic meals. Drones buzz overhead, their cameras scanning the crowds. A vendor cart sells hot noodles, steam rising into the polluted air. The alley is to the west, the metro station is to the north, and a night market sprawls to the east.",
            Exits = new Dictionary<string, string>
            {
                ["west"] = "alley",
                ["north"] = "metro",
                ["east"] = "market"
            },
            Items = new List<string>()
        };

        state.Rooms["metro"] = new Room
        {
            Id = "metro",
            Name = "Metro Station Entrance",
            ShortDescription = "A metro station entrance with turnstiles and a security booth.",
            Description = "Turnstiles block access to the underground. A bored security guard watches from behind bulletproof glass. A sign reads 'Fare: 10 credits'. The boulevard is to the south and the platform is beyond the turnstiles to the north.",
            Exits = new Dictionary<string, string>
            {
                ["south"] = "street",
                ["north"] = "platform"
            },
            GatedExits = new Dictionary<string, ExitGate>
            {
                ["north"] = new ExitGate
                {
                    RequiresFlag = "metro_paid",
                    FailureMessage = "The turnstile blocks your way. You need to pay the 10 credit fare first."
                }
            },
            Items = new List<string> { "transit-map" }
        };

        state.Rooms["platform"] = new Room
        {
            Id = "platform",
            Name = "Sector 7 Platform",
            ShortDescription = "An underground platform humming with distant trains.",
            Description = "The underground platform hums with the vibration of distant trains. Flickering fluorescent lights cast a sickly glow over cracked tiles. A few figures wait in the shadows. A corridor to the east is marked with a faded red cross. The turnstiles are to the south.",
            Exits = new Dictionary<string, string>
            {
                ["south"] = "metro",
                ["east"] = "clinic"
            },
            Items = new List<string>()
        };

        state.Rooms["clinic"] = new Room
        {
            Id = "clinic",
            Name = "Underground Clinic",
            ShortDescription = "A makeshift clinic tucked into a corridor, smelling of antiseptic.",
            Description = "Medical equipment of questionable origin lines the walls. The air smells of antiseptic and solder. A woman with sharp eyes and steady hands works at a cluttered bench, surrounded by neural implant components. A sign on the wall reads 'Kira's Clinic - Repairs, Upgrades, Memory Work'. The platform is to the west.",
            Exits = new Dictionary<string, string>
            {
                ["west"] = "platform"
            },
            Items = new List<string> { "neural-interface" }
        };

        state.Rooms["market"] = new Room
        {
            Id = "market",
            Name = "Night Market",
            ShortDescription = "A cramped night market packed with salvage stalls and street food.",
            Description = "Cramped stalls packed with salvaged tech, bootleg software, and street food. Cables hang overhead like vines. Vendors call out prices in a mix of languages. A narrow passage leads west back to the boulevard.",
            Exits = new Dictionary<string, string>
            {
                ["west"] = "street"
            },
            Items = new List<string> { "cortex-chip" }
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
                    Label = "Ask about work",
                    RequiresFlag = "met_chrome",
                    Text = "I could use someone to run a package to the metro station. 10 credits. Interested?",
                    SetsFlag = "job_offered",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "chrome_ask_kreznik",
                    Label = "Ask about Kreznik",
                    RequiresFlag = "met_chrome",
                    Text = "Kreznik? It's a district run by NeoCortex Corp. They own the buildings, the metro, the air you breathe. People come here because it's cheap. People stay because they can't afford to leave.",
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
                    Id = "chrome_kira_hint",
                    Label = "Ask about the datapad",
                    RequiresFlag = "job_paid",
                    RequiresItem = "datapad",
                    Text = "That datapad you've got... I've seen those before. Memory extraction logs. If you want answers, find someone who does neural work. I've heard there's a clinic down in the metro tunnels.",
                    SetsFlag = "chrome_mentioned_kira",
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

        state.Npcs["vendor"] = new Npc
        {
            Id = "vendor",
            Name = "Noodle Vendor",
            Description = "An old man with grease-stained hands and a warm smile. His cart is a mobile kitchen of hissing pipes and bubbling pots.",
            RoomId = "street",
            Dialogue = new List<DialogueLine>
            {
                new DialogueLine
                {
                    Id = "vendor_intro",
                    Text = "Hot noodles! Best in Kreznik! ...You look lost, friend. New to the sector?",
                    SetsFlag = "met_vendor",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "vendor_kira_hint",
                    Label = "Ask about Kira",
                    RequiresFlag = "read_datapad",
                    Text = "Kira? Yeah, I know Kira. She runs a clinic down in the metro tunnels, past the Sector 7 platform. Does memory work, neural stuff. Good with her hands. Just don't ask what she charges.",
                    SetsFlag = "vendor_mentioned_kira",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "vendor_ask_rumors",
                    Label = "Ask about rumors",
                    RequiresFlag = "met_vendor",
                    Text = "Word on the street is NeoCortex is running some new project. 'Icarus', they call it. People go in, they don't come out the same. But you didn't hear that from me.",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "vendor_idle",
                    RequiresFlag = "met_vendor",
                    Text = "Noodles are getting cold. You buying or browsing?",
                    Repeatable = true
                }
            }
        };

        state.Npcs["kira"] = new Npc
        {
            Id = "kira",
            Name = "Kira",
            Description = "A woman in her thirties with precise, augmented hands and scanning-lens eyes. She moves with the focused calm of someone used to working inside people's heads.",
            RoomId = "clinic",
            Dialogue = new List<DialogueLine>
            {
                new DialogueLine
                {
                    Id = "kira_intro",
                    Text = "Another patient? Or just lost? This is a clinic, not a tourist stop. I'm Kira.",
                    SetsFlag = "met_kira",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "kira_datapad_reaction",
                    Label = "Show the datapad",
                    RequiresFlag = "met_kira",
                    RequiresItem = "datapad",
                    Text = "That datapad... let me see. These are fragments of a memory extraction log. YOUR memory extraction log. Someone wiped your memories deliberately. I can reconstruct them, but I need a cortex chip - military grade. You won't find one in any legit shop.",
                    SetsFlag = "kira_quest_offered",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "kira_quest_accept",
                    RequiresFlag = "kira_quest_offered",
                    Text = "Find me a cortex chip. Try the night market east of Neon Boulevard - sometimes military surplus shows up there. Bring it to me and we'll start unlocking what's in your head.",
                    SetsFlag = "kira_quest_accepted",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "kira_chip_delivery",
                    RequiresFlag = "kira_quest_accepted",
                    RequiresItem = "cortex-chip",
                    Text = "A cortex chip. This will do. Sit down and try not to move. ...There. I've started the reconstruction. Fragments are surfacing: a corporate lab, a name - 'Project Icarus', armed guards... This is bigger than a street-level wipe. Come back after your mind settles. There's more to recover.",
                    SetsFlag = "memory_restored",
                    RemovesItem = "cortex-chip",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "kira_ask_about_self",
                    Label = "Ask about Kira",
                    RequiresFlag = "met_kira",
                    Text = "I used to work for NeoCortex. Neural research division. I left when I saw what they were doing to people. Now I fix what they break. That's all you need to know.",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "kira_ask_neocortex",
                    Label = "Ask about NeoCortex",
                    RequiresFlag = "met_kira",
                    Text = "NeoCortex runs Kreznik, top to bottom. They've got their fingers in everything - neural implants, surveillance, memory tech. Stay off their radar if you can.",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "kira_after_restore",
                    RequiresFlag = "memory_restored",
                    Text = "Your neural pathways are still settling. I can see more fragments forming. We'll need deeper access next time. For now, be careful - whoever wiped you did it for a reason.",
                    Repeatable = true
                },
                new DialogueLine
                {
                    Id = "kira_waiting",
                    RequiresFlag = "kira_quest_accepted",
                    Text = "Still need that cortex chip. Try the night market on Neon Boulevard, east side.",
                    Repeatable = true
                },
                new DialogueLine
                {
                    Id = "kira_generic",
                    RequiresFlag = "met_kira",
                    Text = "I do neural repairs, memory reconstruction, that sort of thing. If you have something for me to look at, we can talk.",
                    Repeatable = true
                }
            }
        };

        state.LastMessage = "You wake up in a dark alley. Your head pounds. You don't remember how you got here. A flickering neon sign above reads 'Welcome to Kreznik' - but that doesn't help. Everyone knows Kreznik.";

        return state;
    }
}

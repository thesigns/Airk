using Airk.World;

namespace Airk.State;

public static class WorldBuilder
{
    private const string RegulaminText =
        "=== KREZNIK METRO TRANSIT AUTHORITY ===\n" +
        "REGULATION NOTICE\n\n" +
        "1. Entry fee: 5 credits, deducted automatically at the\n" +
        "   turnstile. Re-entry is permitted within the same day.\n\n" +
        "2. To travel between stations, use: travel <station-code>\n\n" +
        "3. Active stations:\n" +
        "   RED LINE: Sector 7 [s7] --- Outer Rim [rim]\n" +
        "   BLUE LINE: Service suspended until further notice.\n\n" +
        "4. NeoCortex Corp assumes no liability for delays,\n" +
        "   cancellations, or incidents on metro property.\n\n" +
        "=== TRAVEL SAFE. TRAVEL MONITORED. ===";

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
            Description = "Holographic advertisements tower above, selling everything from neural upgrades to synthetic meals. Drones buzz overhead, their cameras scanning the crowds. A vendor cart sells hot noodles, steam rising into the polluted air. A regulation notice is posted by the metro turnstile to the north. The alley is to the west and a night market sprawls to the east.",
            Exits = new Dictionary<string, string>
            {
                ["west"] = "alley",
                ["north"] = "metro",
                ["east"] = "market"
            },
            ExitCosts = new Dictionary<string, int>
            {
                ["north"] = 5
            },
            Readables = new Dictionary<string, string>
            {
                ["regulamin"] = RegulaminText,
                ["regulations"] = RegulaminText
            },
            Items = new List<string>()
        };

        state.Rooms["metro"] = new Room
        {
            Id = "metro",
            Name = "Metro Station Entrance",
            ShortDescription = "A metro station entrance with turnstiles and a security booth.",
            Description = "Past the turnstile, you stand in the metro station entrance hall. A bored security guard watches from behind bulletproof glass. The boulevard is back to the south and the platform is to the north.",
            Exits = new Dictionary<string, string>
            {
                ["south"] = "street",
                ["north"] = "platform"
            },
            Items = new List<string> { "transit-map" }
        };

        state.Rooms["platform"] = new Room
        {
            Id = "platform",
            Name = "Sector 7 Platform",
            ShortDescription = "An underground platform humming with distant trains.",
            Description = "The underground platform hums with the vibration of distant trains. Flickering fluorescent lights cast a sickly glow over cracked tiles. A few figures wait in the shadows. A corridor to the east is marked with a faded red cross. The turnstiles are to the south. A battered regulation sign hangs on the wall near the tracks. A display board shows: Red Line - Sector 7 [s7].",
            Exits = new Dictionary<string, string>
            {
                ["south"] = "metro",
                ["east"] = "clinic"
            },
            Readables = new Dictionary<string, string>
            {
                ["regulamin"] = RegulaminText,
                ["regulations"] = RegulaminText
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
                    Text = "I could use someone to run a package to the metro station. 50 credits. Interested?",
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
                    Id = "chrome_job_done_tampered",
                    RequiresFlag = "job_complete_tampered",
                    Text = "The guard told me you opened the package. I said don't open it. You get nothing. Consider it a lesson in following instructions.",
                    SetsFlag = "job_paid",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "chrome_job_done",
                    RequiresFlag = "job_complete",
                    Text = "Nice work. Here's your 50 credits. Maybe I'll have more work for you later.",
                    GivesCredits = 50,
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
                    Id = "chrome_ns7_confession",
                    Label = "Ask about the neural suppressor",
                    RequiresFlag = "job_complete_tampered",
                    Text = "You saw what was in the package? An NS-7. I've been running those for months. I don't ask who they're for. But lately... the orders have been getting bigger. Industrial scale. Whatever NeoCortex is building, they need a lot of people to forget.",
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
                    Id = "guard_package_opened",
                    RequiresFlag = "opened_package",
                    RequiresItem = "package",
                    Text = "Package from Chrome? ...Wait. You opened it? That wasn't part of the deal. I'll take it, but Chrome's going to hear about this.",
                    SetsFlag = "job_complete_tampered",
                    RemovesItem = "package",
                    Repeatable = false
                },
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
                    Id = "guard_idle",
                    Text = "Move along. Platform's to the north.",
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
                    Id = "kira_ns7_reaction",
                    Label = "Tell her about the neural suppressor",
                    RequiresFlag = "opened_package",
                    Text = "An NS-7 Neural Suppressor? NeoCortex makes those. Military grade memory wipes. " +
                           "That's the same technology they used on you. If someone in this district is " +
                           "receiving shipments of those... it means they're still running operations here. " +
                           "And your bartender friend is part of the supply chain, whether she knows it or not.",
                    SetsFlag = "kira_knows_ns7",
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
                    Id = "kira_after_deep_scan",
                    RequiresFlag = "deep_scan_done",
                    Text = "You know who you are now. A NeoCortex researcher who tried to do the right thing. They'll come for you again if they find out you're remembering. Be ready.",
                    Repeatable = true
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

        state.Npcs["scavenger"] = new Npc
        {
            Id = "scavenger",
            Name = "Scavenger",
            Description = "A wiry figure in a patched-up coat, fingers stained with solder. She picks through a pile of circuit boards, testing each one with a handheld scanner.",
            RoomId = "market",
            Dialogue = new List<DialogueLine>
            {
                new DialogueLine
                {
                    Id = "scav_intro",
                    Text = "Hey, hands off the merchandise. ...Oh, you're buying? Different story. I'm Patch. I deal in surplus -- military, corporate, whatever falls off a transport.",
                    SetsFlag = "met_scavenger",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "scav_ask_cortex",
                    Label = "Ask about cortex chips",
                    RequiresFlag = "met_scavenger",
                    Text = "Cortex chips? Military grade, hard to come by. That one over there's the last I've got. Fell off a NeoCortex shipment headed for their research wing. Don't ask how I got it.",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "scav_ask_neocortex",
                    Label = "Ask about NeoCortex shipments",
                    RequiresFlag = "met_scavenger",
                    Text = "NeoCortex moves a lot of hardware through Kreznik. Neural implants, memory modules, stuff I can't even identify. Lately there's been more of it -- like they're building something big underground.",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "scav_icarus",
                    Label = "Ask about Project Icarus",
                    RequiresFlag = "memory_restored",
                    Text = "Icarus? Keep your voice down. I've seen crates with that label. They go to a facility past the Outer Rim station -- nobody who works there talks about it. One guy tried. They found him wiped clean, wandering the streets. Sound familiar?",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "scav_idle",
                    RequiresFlag = "met_scavenger",
                    Text = "Browse all you want. Just don't break anything.",
                    Repeatable = true
                }
            }
        };

        state.Npcs["drifter"] = new Npc
        {
            Id = "drifter",
            Name = "Drifter",
            Description = "A gaunt man sitting against the wall, eyes unfocused. His clothes are threadbare, but the neural port behind his ear looks brand new -- and expensive.",
            RoomId = "platform",
            Dialogue = new List<DialogueLine>
            {
                new DialogueLine
                {
                    Id = "drifter_intro",
                    Text = "...don't touch me. I'm fine. Just... sitting. Waiting for a train that never comes.",
                    SetsFlag = "met_drifter",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "drifter_ask_memories",
                    Label = "Ask what happened to him",
                    RequiresFlag = "met_drifter",
                    Text = "I had a life. I think. There are shapes where memories should be. A lab. White lights. They said I volunteered, but I don't remember volunteering for anything.",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "drifter_ask_lab",
                    Label = "Ask about the lab",
                    RequiresFlag = "met_drifter",
                    Text = "Past the Outer Rim station. Underground. I remember the smell -- ozone and antiseptic. And a name on the door. 'Icarus'. That's all I've got.",
                    SetsFlag = "drifter_mentioned_icarus",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "drifter_post_restore",
                    Label = "Tell him about your memories",
                    RequiresFlag = "memory_restored",
                    Text = "You too? Then they're still doing it. Still wiping people. I thought maybe it stopped. ...Be careful. If they find out you're remembering, they'll come for you again.",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "drifter_idle",
                    RequiresFlag = "met_drifter",
                    Text = "...still waiting.",
                    Repeatable = true
                }
            }
        };

        // Outer Rim sector

        state.Rooms["outerrim-platform"] = new Room
        {
            Id = "outerrim-platform",
            Name = "Outer Rim Platform",
            ShortDescription = "A dimly lit platform at the edge of the metro network.",
            Description = "The platform here is barely maintained. Cracked tiles and exposed wiring line the walls. The fluorescent lights flicker erratically. A heavy security door to the north is marked 'RESTRICTED'. A passage to the east leads to a maintenance corridor. A regulation sign hangs crookedly on the wall. A display board shows: Red Line - Outer Rim [rim].",
            Exits = new Dictionary<string, string>
            {
                ["north"] = "facility-gate",
                ["east"] = "maintenance"
            },
            Readables = new Dictionary<string, string>
            {
                ["regulamin"] = RegulaminText,
                ["regulations"] = RegulaminText
            },
            Items = new List<string>()
        };

        state.Rooms["facility-gate"] = new Room
        {
            Id = "facility-gate",
            Name = "Facility Gate",
            ShortDescription = "A fortified checkpoint guarding access to a NeoCortex facility.",
            Description = "A reinforced security checkpoint. Cameras track your movement. A heavy blast door blocks the way north, sealed with a biometric lock. A terminal on the wall displays: 'ICARUS RESEARCH FACILITY - LEVEL 5 CLEARANCE REQUIRED'. The platform is to the south.",
            Exits = new Dictionary<string, string>
            {
                ["south"] = "outerrim-platform"
            },
            Items = new List<string>()
        };

        state.Rooms["maintenance"] = new Room
        {
            Id = "maintenance",
            Name = "Maintenance Corridor",
            ShortDescription = "A narrow service corridor cluttered with old equipment.",
            Description = "Pipes run along the low ceiling, dripping condensation. Old metro maintenance equipment is piled against the walls. Someone has set up a makeshift living space in a recessed alcove -- a cot, a portable heater, and stacks of data storage cubes. The platform is to the west.",
            Exits = new Dictionary<string, string>
            {
                ["west"] = "outerrim-platform"
            },
            Items = new List<string> { "access-card" }
        };

        state.Npcs["echo"] = new Npc
        {
            Id = "echo",
            Name = "Echo",
            Description = "A nervous woman in a grimy NeoCortex maintenance uniform. She flinches at every sound from the tunnels.",
            RoomId = "maintenance",
            Dialogue = new List<DialogueLine>
            {
                new DialogueLine
                {
                    Id = "echo_intro",
                    Text = "Don't-- don't come any closer. ...Wait. You're not security. Who are you? Never mind. I'm Echo. I used to work maintenance at the Icarus facility up north. I got out. Barely.",
                    SetsFlag = "met_echo",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "echo_ask_facility",
                    Label = "Ask about the Icarus facility",
                    RequiresFlag = "met_echo",
                    Text = "Level 5 clearance to get in. Biometric locks, armed guards, the works. They're running neural experiments in there. Mass scale. I've seen the processing rooms -- hundreds of chairs, all wired up. They call it 'population optimization'.",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "echo_ask_access",
                    Label = "Ask how to get inside",
                    RequiresFlag = "met_echo",
                    Text = "The blast door needs Level 5 biometrics. But there's a maintenance access -- old tunnel, sealed years ago. My access card might still work on the service entrance. ...Take it. I'm not going back in there.",
                    SetsFlag = "echo_offered_card",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "echo_give_card",
                    RequiresFlag = "echo_offered_card",
                    Text = "The card is right there, on the cot. It's a maintenance-level pass. Won't get you into the main labs, but it'll get you through the service tunnels. Be careful -- the automated security doesn't ask questions.",
                    SetsFlag = "echo_gave_card_info",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "echo_post_deep_scan",
                    Label = "Tell her you worked at Icarus",
                    RequiresFlag = "deep_scan_done",
                    Text = "You worked there? As a researcher? Then you know what they're doing. And they wiped you for trying to stop it. ...Listen. There's a server room in the facility. If you can get to it, you could pull the evidence. Expose everything. But you'd need someone on the outside ready to broadcast it.",
                    SetsFlag = "echo_knows_identity",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "echo_idle",
                    RequiresFlag = "met_echo",
                    Text = "I'm not going back. But if you're crazy enough to try... good luck.",
                    Repeatable = true
                }
            }
        };

        state.Npcs["rimguard"] = new Npc
        {
            Id = "rimguard",
            Name = "Facility Guard",
            Description = "A guard in heavy NeoCortex tactical armor, face hidden behind a reflective visor. An automatic rifle is slung across their chest.",
            RoomId = "facility-gate",
            Dialogue = new List<DialogueLine>
            {
                new DialogueLine
                {
                    Id = "rimguard_intro",
                    Text = "Restricted area. Turn around.",
                    SetsFlag = "met_rimguard",
                    Repeatable = false
                },
                new DialogueLine
                {
                    Id = "rimguard_no_access",
                    RequiresFlag = "met_rimguard",
                    Text = "I said restricted. No clearance, no entry. That's how it works.",
                    Repeatable = true
                }
            }
        };

        // Metro stations

        state.MetroStations["s7"] = new MetroStation
        {
            Code = "s7",
            Name = "Sector 7 Platform",
            PlatformRoomId = "platform",
            Line = "Red"
        };

        state.MetroStations["rim"] = new MetroStation
        {
            Code = "rim",
            Name = "Outer Rim",
            PlatformRoomId = "outerrim-platform",
            Line = "Red"
        };

        state.LastMessage = "You wake up in a dark alley. Your head pounds. You don't remember how you got here. A flickering neon sign above reads 'Welcome to Kreznik' - but that doesn't help. Everyone knows Kreznik.";

        return state;
    }
}

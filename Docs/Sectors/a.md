# Sector 7 — Street Level

Starting zone. Neon-lit alleys, dive bars, street markets, cheap noodle stands.
Low difficulty. Where new players learn the ropes.

53 rooms, 8 sub-areas.

---

## Map

```
        a26+a25+a27             a44                 a63
         +   +                   +                   -
        a24 a23             a42+a43             a61+a62
         +   +               +                   +
        a22+a20+a21         a41+a40             a60
             +               +                   +
    a10+a11+a12+a13+a14+a15+a16+a17+a18+a19
     +           +               +           +
    a06+a01     a30             a50+a51     a70
     +   +       +               +   +       +
    a05 a02 a32+a31+a33         a52 a53 a71+a72
     +   +   +   +               +   +   +   +
    a04+a03 a35 a34             a54+a55 a73-a74
     +           +                   +
    a00         a36                 a56
```

`+` open passage, `-` gated passage

---

## Connections

```
ID    N     S     E     W     Sub-area / Name
----  ----  ----  ----  ----  -------------------------
a00   a04   .     .     .     Alleys / Starting Alley
a01   .     a02   .     a06   Alleys / Narrow Passage
a02   a01   a03   .     .     Alleys / Dumpster Alley
a03   a02   .     .     a04   Alleys / Dead End
a04   a05   a00   a03   .     Alleys / Alley Junction
a05   a06   a04   .     .     Alleys / Fire Escape Alley
a06   a10   a05   a01   .     Alleys / Alley Mouth

a10   .     a06   a11   .     Main Street / West End
a11   .     .     a12   a10   Main Street / Noodle Stand
a12   a20   .     a13   a11   Main Street / Bar Corner
a13   .     a30   a14   a12   Main Street / Market Gate
a14   .     .     a15   a13   Main Street / Street Center
a15   a41   .     a16   a14   Main Street / Metro Entrance
a16   .     a50   a17   a15   Main Street / Residential Gate
a17   .     .     a18   a16   Main Street / East Street
a18   a60   .     a19   a17   Main Street / Clinic Road
a19   .     a70   .     a18   Main Street / Warehouse Road

a20   a23   a12   a21   a22   Bar District / Bar Street
a21   .     .     .     a20   Bar District / The Rusty Nail
a22   a24   .     a20   .     Bar District / Side Passage
a23   a25   a20   .     .     Bar District / Neon Row
a24   a26   a22   .     .     Bar District / Upper Passage
a25   .     a23   a27   a26   Bar District / Chrome's Bar
a26   .     a24   a25   .     Bar District / VIP Corner
a27   .     .     .     a25   Bar District / Back Storage

a30   a13   a31   .     .     Night Market / Market Entrance
a31   a30   a34   a33   a32   Night Market / Market Square
a32   .     a35   a31   .     Night Market / Weapons Stall
a33   .     .     .     a31   Night Market / Electronics Stand
a34   a31   a36   .     .     Night Market / Food Court
a35   a32   .     .     .     Night Market / Smuggler's Corner
a36   a34   .     .     .     Night Market / Market Basement

a40   .     .     .     a41   Metro / Platform
a41   a42   a15   a40   .     Metro / Corridor
a42   .     a41   a43   .     Metro / Mezzanine
a43   a44   .     .     a42   Metro / Ticket Hall
a44   .     a43   .     .     Metro / Security Office

a50   a16   a52   a51   .     Residential / Tower Lobby
a51   .     a53   .     a50   Residential / Coffin Hotel
a52   a50   a54   .     .     Residential / Apartment Corridor
a53   a51   a55   .     .     Residential / Laundry Room
a54   a52   .     a55   .     Residential / Stairwell
a55   a53   a56   .     a54   Residential / Upper Corridor
a56   a55   .     .     .     Residential / Basement

a60   a61   a18   .     .     Clinic / Clinic Approach
a61   .     a60   a62   .     Clinic / Waiting Room
a62  [a63]  .     .     a61   Clinic / Treatment Room
a63   .    [a62]  .     .     Clinic / Private Lab

a70   a19   a71   .     .     Warehouse / Entrance
a71   a70   a73   a72   .     Warehouse / Loading Dock
a72   .     a74   .     a71   Warehouse / Storage Floor
a73   a71   .    [a74]  .     Warehouse / Security Room
a74   a72   .     .    [a73]  Warehouse / Locked Storage
```

`[target]` = gated exit (requires flag/item to pass)

---

## Sub-areas

### Alleys (a00-a06) — Starting Area

Dark back alleys behind the main street buildings. Garbage, puddles, flickering
lights. This is where the player wakes up. Low danger, tutorial feel. Teaches
basic navigation and item pickup.

- **a00 Starting Alley** — Dead end. Dumpsters, a single flickering light, wet concrete. You wake up here with nothing. PlayerRespawn point.
- **a01 Narrow Passage** — Tight squeeze between buildings. Pipes overhead dripping condensation. Leads west to the alley mouth.
- **a02 Dumpster Alley** — Overflowing dumpsters. Rats scurry in the shadows. Sometimes useful scraps among the garbage.
- **a03 Dead End** — Graffiti-covered wall. Faint hum of machinery behind it. Someone tagged "THEY SEE EVERYTHING" in red paint.
- **a04 Alley Junction** — Four-way intersection of narrow alleys. A broken vending machine flickers in the corner.
- **a05 Fire Escape Alley** — Rusty fire escape ladders hang overhead. Water drips from humming AC units. Echo of footsteps from above.
- **a06 Alley Mouth** — Where the alleys open onto the main street. Light spills in. The noise of the city hits you.

### Main Street (a10-a19) — Central Corridor

East-west artery connecting all sub-areas. Neon signs, foot traffic, street
vendors. 10 rooms forming the backbone of the sector. The player will pass
through here constantly.

- **a10 West End** — Western dead end. Crumbling building facades. A boarded-up storefront. Alley entrance to the south.
- **a11 Noodle Stand** — Street vendor selling cheap ramen under a tarp. Warm steam rises. Regulars slurp noodles on plastic stools.
- **a12 Bar Corner** — Intersection. Neon signs point north: "BARS", "LIVE MUSIC", "CHEAP DRINKS". Bass thumps from somewhere above.
- **a13 Market Gate** — Colorful lights to the south. Vendors call out deals. A hanging banner reads "NIGHT MARKET — OPEN ALWAYS".
- **a14 Street Center** — Busiest stretch. Holographic ads flicker overhead. A crowd of people moving in both directions.
- **a15 Metro Entrance** — Metro sign to the north. Turnstile sounds. A transit map is posted on the wall. Regulamin readable.
- **a16 Residential Gate** — Quieter stretch. Residential towers loom to the south. Security cameras mounted above.
- **a17 East Street** — Thinning crowds. Fewer working streetlights. The buildings look more worn, grittier.
- **a18 Clinic Road** — A medical cross sign flickers to the north. Faint antiseptic smell mixes with street air.
- **a19 Warehouse Road** — Eastern dead end. Industrial feel. Chain-link fences and loading areas to the south.

### Bar District (a20-a27) — Social Hub

Neon-soaked entertainment zone north of Main Street. Bars, back rooms, private
corners. NPCs here have information, jobs, and rumors. Chrome's home turf.

- **a20 Bar Street** — Main drag. Neon everywhere. Music bleeds from every doorway. The smell of synth-alcohol and smoke.
- **a21 The Rusty Nail** — Dive bar interior. Sticky floor, cheap drinks, a cracked mirror behind the counter. Regulars who know things.
- **a22 Side Passage** — Narrow passage between bars. Empty bottles, graffiti, a sleeping drunk propped against the wall.
- **a23 Neon Row** — Slightly upscale strip. Brighter signs, cocktail bars, slightly less sticky floors. Posers and wannabes.
- **a24 Upper Passage** — Narrow stairway between levels. Cables run along the walls. Smells like ozone and old beer.
- **a25 Chrome's Bar** — The bar. Dim lighting, good sight lines, a back exit for quick escapes. Chrome's territory.
- **a26 VIP Corner** — Curtained alcove area. Quieter than the rest. For private conversations and discreet deals.
- **a27 Back Storage** — Behind Chrome's bar. Crates, bottles, and things Chrome doesn't talk about.

### Night Market (a30-a36) — Commerce Hub

Open-air market south of Main Street in a covered courtyard. Stalls, vendors,
street food. Primary shopping area in Sector 7. Economy-focused.

- **a30 Market Entrance** — Archway with hanging lanterns. Smell of cooking food and ozone. A hand-painted sign: "ALL SALES FINAL".
- **a31 Market Square** — Central hub. Stalls on all sides. Noisy, crowded, colorful. Everyone is buying, selling, or stealing.
- **a32 Weapons Stall** — "Self-defense equipment." Knives, pipes, shock batons, and things that definitely aren't legal.
- **a33 Electronics Stand** — Salvaged tech, data chips, second-hand neural gear. The vendor tests everything in front of you.
- **a34 Food Court** — Cheap food stalls. Synth-meat skewers, protein bars, questionable noodles. Better than starving.
- **a35 Smuggler's Corner** — Quiet stall tucked in the back. The vendor speaks softly and has connections. Discretion guaranteed.
- **a36 Market Basement** — Below the market floor. Storage, rats, and inventory that fell off trucks. Damp and dark.

### Metro Station (a40-a44) — Transit Hub

Sector 7's metro station. Multi-level, somewhat maintained. Gateway to other
sectors via the Red Line. Entry fee applies from the street.

- **a40 Platform** — Red Line platform. Trains arrive and depart with a rush of stale air. Departure boards flicker. Travel point.
- **a41 Metro Corridor** — Tiled corridor connecting platform to station. Regulamin posted on the wall. Fluorescent lights buzz.
- **a42 Mezzanine** — Upper level. Vending machines, a metal bench, transit information screens showing delays.
- **a43 Ticket Hall** — Main station hall. Turnstiles, ticket machines. A bored attendant behind scratched plexiglass.
- **a44 Security Office** — Small office with banks of monitors. Metro security spends most of their shift here.

### Residential (a50-a56) — Housing Area

Residential towers and coffin hotels south of Main Street. Where people of
Sector 7 live, or try to. Housing progression area — coffin hotel through
apartments.

- **a50 Tower Lobby** — Building entrance. Dented mailboxes line the wall. Elevator has an "OUT OF ORDER" sign that looks permanent.
- **a51 Coffin Hotel** — Rows of sleeping pods stacked three high. Cheap. Functional. A red LED means occupied. Starter housing.
- **a52 Apartment Corridor** — Long hallway, numbered doors. Muffled sounds behind thin walls. Cooking smells, arguments, silence.
- **a53 Laundry Room** — Industrial washers, one perpetually broken. A notice board with handwritten ads and missing persons flyers.
- **a54 Stairwell** — Concrete stairs between floors. Echoing footsteps. Emergency lighting casts everything in pale green.
- **a55 Upper Corridor** — Higher floor. Slightly nicer apartments. A grimy window offers a view of neon-lit rooftops.
- **a56 Basement** — Building basement. Wire storage cages, utility pipes, a locked service door leading somewhere unknown.

### Clinic (a60-a63) — Medical Area

Free clinic and surroundings north of East Street. Kira's territory. HP/SP
healing services, cyberware maintenance. Quiet, tense, underfunded.

- **a60 Clinic Approach** — Narrow street. Medical waste bins along the wall. A dim blue cross sign above a doorway.
- **a61 Waiting Room** — Plastic chairs, flickering fluorescent light. A few people sit quietly, avoiding eye contact.
- **a62 Treatment Room** — Medical equipment, a cot, vital monitors. Clean but worn. Kira works here.
- **a63 Private Lab** — Behind a locked door. Kira's personal research space. Restricted access. (Gated: requires flag)

### Warehouse (a70-a74) — Danger Zone

Abandoned and semi-abandoned warehouses south of East Street. Higher danger
than the rest of Sector 7. Better loot. The edge where the sector frays.

- **a70 Warehouse Entrance** — Chain-link gate, partially bent open. "KEEP OUT" signs. Doesn't look safe. Smells like rust and chemicals.
- **a71 Loading Dock** — Raised concrete platform. Empty crates, an abandoned forklift. Tire marks in the dust.
- **a72 Storage Floor** — Vast open floor. Metal shelving units, some collapsed. Shadows pool between the rows.
- **a73 Security Room** — Small room with dead monitors and a smashed console. Someone was guarding something here once.
- **a74 Locked Storage** — Heavy door with an electronic lock. Red light. Whatever's inside, someone wanted it kept. (Gated: requires flag/item)

---

## Key Locations

| Function        | Room | Notes                                    |
|-----------------|------|------------------------------------------|
| PlayerRespawn   | a00  | Starting alley, default spawn            |
| Metro Platform  | a40  | Red Line, travel to other sectors        |
| Shop (weapons)  | a32  | Weapons stall                            |
| Shop (tech)     | a33  | Electronics stand                        |
| Shop (misc)     | a35  | Smuggler — special inventory             |
| Clinic services | a62  | HP/SP healing, cyberware                 |
| Housing (basic) | a51  | Coffin hotel — cheapest tier             |
| Housing (mid)   | a52  | Apartments — mid tier                    |
| Gated exit      | a62→a63 | Kira's private lab                    |
| Gated exit      | a73→a74 | Locked warehouse storage              |

# Design

Game systems design for Airk — a single-player cyberpunk MUD.

---

## Attributes

Five stats, all 4-letter names. No class system — build through stat allocation and equipment.

| Stat | Domain | Combat role | Out-of-combat role |
|------|--------|------------|-------------------|
| **Body** | Physical | Melee damage, max HP | Carry capacity, intimidation |
| **Tech** | Smart vs machines | Device/hack attacks | Crafting, electronic bypass, cyberware use |
| **Edge** | Agility | Ranged accuracy, dodge, initiative | Stealth, awareness |
| **Wits** | Smart vs people | Resist manipulation | Dialogue, bargaining, perception |
| **Luck** | Chance | Crit chance, escape rolls | Loot quality, hidden discoveries |

**Character creation**: base 3 in each stat, +5 points to distribute.
**Level up**: +1 stat point per level.

---

## Health: Dual Pool

### HP (Hit Points)
- Physical health. Base = 50 + Body * 10.
- Reduced by: combat damage, environmental hazards.
- 0 HP = death. Respawn at nearest respawn point, lose 20% credits.
- Recovery: slow passive regen, healing items, clinic services.

### SP (Sanity Points)
- Mental health. Base = 50 + Wits * 10.
- Reduced by:
  - **Cyberware installation** (permanent max SP reduction per implant)
  - Neural attacks / hacking
  - Traumatic events
- 0 SP = cyberpsychosis: permanent debuff until treated at clinic (expensive).
- Recovery: clinic (expensive, large restore) or street drugs (cheap, small restore, builds addiction).

### Addiction
- Street drug use increments a tolerance counter.
- Above threshold: periodic Wits/Edge debuffs when not using.
- Tolerance keeps rising with use — spiraling cost, need more frequent doses.
- Classic cyberpunk trap: start using to save money, end up spending more than the clinic.

### Cyberware Trade-off
- Each implant permanently lowers max SP.
- More chrome = more powerful, but closer to the edge.
- Full chrome (5/5 slots) puts you near cyberpsychosis threshold.
- Player must choose: which slots to fill, which to leave empty for safety.

---

## Combat

Turn-based. Initiated with `attack <target>`.

1. **Initiative**: Edge determines who strikes first.
2. **Attack roll**: Body (melee weapon) or Edge (ranged weapon) vs target defense.
3. **Dodge**: Edge-based chance to avoid the hit entirely.
4. **Damage**: weapon base damage + stat modifier - target armor.
5. **Critical hits**: Luck-based chance for bonus damage.
6. **Enemy turn**: mob attacks back using same formula.
7. Repeat until one side is at 0 HP or flees.

**Fleeing**: `flee` command. Edge + Luck check. Failure means you take a hit and stay. Success moves you to a random adjacent room.

**Death**: 0 HP. Respawn at respawn point, lose 20% credits. Inventory preserved.

---

## Equipment

### Weapon (1 slot)
- Melee weapons: scale with Body. Knives, pipes, cyberblades.
- Ranged weapons: scale with Edge. Pistols, SMGs, rifles.
- Player carries one equipped weapon. Can swap from inventory.

### Armor (1 slot)
- Flat damage reduction per hit.
- Heavier armor may have stat penalties (Edge reduction).

### Cyberware (5 slots)
One implant per body location. Each permanently reduces max SP on install.

| Slot | Location | Example bonuses |
|------|----------|----------------|
| **Neural** | Head | Hacking power, perception, resist manipulation |
| **Optics** | Eyes | Ranged accuracy, scanning, night vision |
| **Arms** | Arms | Melee damage, crafting precision, grip strength |
| **Legs** | Legs | Dodge bonus, flee chance, stealth |
| **Spine** | Spine | Max HP boost, armor, endurance |

**Level requirements**: higher-tier equipment requires minimum character level or stats.

---

## Mobs

Hostile NPCs, separate from dialogue NPCs.

**Defined by**: stats (Body/Tech/Edge), weapon, armor, loot table, respawn timer, behavior type.

**Behavior types**:
- **Aggressive**: attacks player on sight (entering the room).
- **Defensive**: attacks only if attacked first.
- **Patrol**: moves between rooms on a timer.

**Zone difficulty tiers**: mobs in each zone match the zone's difficulty level.

**Boss mobs**: unique, quest-related, don't respawn after defeat. Better loot.

**Respawning**: regular mobs respawn after N turns. Loot refreshes on respawn.

---

## Economy

**Income**:
- Mob drops (credits + items)
- Quest rewards
- Selling items to shops

**Spending**:
- Shops: equipment, cyberware, consumables
- Services: clinic (HP/SP healing, addiction treatment, cyberware install), hacking terminals
- Housing: rent or purchase
- Metro fares

**Shops**: located in safe zones. Stock may be static or rotating.

**Selling**: items sell for a fraction of their buy price.

---

## Progression

- **XP** from: combat kills, quest completion.
- **Level up**: +1 stat point to distribute. Max HP increases with Body, max SP with Wits.
- **No class system**: build identity through stat allocation + equipment + cyberware choices.
- **Soft cap**: XP required per level increases progressively. No hard max.

---

## Housing

Player starts homeless. Housing is an early and ongoing progression goal.

| Tier | Cost | Stash slots | Regen bonus | Notes |
|------|------|------------|-------------|-------|
| Homeless | Free | 0 | None | Starting state |
| Coffin hotel | Cheap rent | 1 | Minimal | Available early |
| Apartment | Medium rent/buy | 3-5 | Moderate HP regen | Mid-game |
| Premium | Expensive | 5-10 | HP + SP regen | Late-game, premium zone locations |

**Functions**:
- **Stash**: safe item storage separate from inventory.
- **Rest**: `rest` command for faster HP regen (and SP regen at higher tiers).
- **Respawn point**: your home overrides zone respawn. Die anywhere → wake up at home.

**Location**: housing available in multiple zones. Player chooses where to live.
**Payment**: rented (recurring credit cost) or bought (one-time, more expensive).

---

## World Structure

### Zone Graph
Zones are nodes, metro lines are edges. The graph serves:
- Metro routing (which zones connect to which)
- Respawn proximity (BFS for nearest zone with respawn point)
- Difficulty mapping (zone tiers)

### Zones
- 30-80 rooms each. Theme and difficulty tier. Built out gradually.
- Each zone has: safe area (hub, shops, quest NPCs) + dangerous areas (mobs, loot).
- Gated areas within zones may require stats, items, or quest flags.
- Inter-zone travel via metro lines between zone hubs.

### Respawn
- Rooms can be tagged `PlayerRespawn`.
- On death: if player has housing → respawn at home.
- No housing: respawn at random `PlayerRespawn` room in current zone.
- Current zone has no respawn: BFS the zone graph for nearest zone with one.
- Player always respawns in the same or nearest zone — never teleported across the city.

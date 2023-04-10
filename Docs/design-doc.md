# Combat Phase Design Document
## Game Scope and Direction
The vision of Combat Phase is to be a tile-based, turn-based game where two players each control a commander, utilizing different units, buildings, and clever positioning in order to either kill the opposing commander or kill enough of its units to win.

**Ideal Time Per Turn:**  ~1 minute  
**Average Game Length:** 30 minutes - 1 hour

**Gameplay Loop:**  
A game starts with an early game phase which will encourage the player to set up their faction in some way, determining what the best position is to attack the enemy and to defend their commander. After this phase, there will be a greater focus on the battlefield. However, the players should still be encouraged to engage with their base of operations. 

The map size should be big enough to put an emphasis on positioning. This also introduces the possibility of making maps more interesting by having different sections with distinguishable features.

---

## System Mechanics
### Currency Economy
**The primary manageable resource is gold.**

**Used to:**
* Recruit Units
* Heal Units
* Enhance Unit Actions

**Resource Taps:**
* Passive income 
* Enhance passive income with buildings
* Unique active income for factions


### Unit Action Tokens
* **Traversal Tokens** - Only usable for movement
* **Action Tokens** - Usable for movement or abilities

By default, each unit has a set amount of these tokens and they regenerate every turn.

**Turn Limiter**  
Each commander has X amount of units they can command per turn, not including themselves. This stops the player from having exceedingly long turns if they have too many units, and adds an element of strategy knowing you cannot manage all your units in the same turn, leading to chess-esque situations. The fact that the commander is not included in this calculation encourages the commander to be used, as making them sit idly puts the player at a disadvantage for not using all resources available to them.

### Win Conditions
* **Killing the Commander**  
* **Unit Kill Count**

Killing your own units does not contribute towards the kill counter, creating a micro-decision where you can finish off your unit before it gets killed, sacrificing a possible extra action you could have taken with that unit. 

### Base of Operations
Each commander starts the game with a base of operations. This is a collection of 4 structures that provide some utility to the player: the **Enlistment Tube**, **Recovery Hut**, **Unit Enhancement Station**, and a **Faction-Exclusive Structure**. The first three of these structures act as the primary consumers of currency, and should be fine-tuned so that there is always a trade-off when choosing to spend gold on one structure over the other.

**Enlistment Tube**  
Operable by an operator unit  
Non-traversable   
When operated, the player is given a list of potential units they can enlist by spending gold. Up to two units may be enlisted per turn. When a unit is enlisted, the player is given the option to enhance them from the get-go.

**Recovery Hut**  
Operable by an operator unit  
Non-traversable  
When operated, the operator unit can pay a price in order to heal a unit within X range by X amount.

**Unit Enhancement Station**  
Operable by an operator unit  
Non-traversable  
When used, the operator unit can choose a unit within X range, and pay a price in order to enhance one of its actions for X amount of uses.

**Exclusive Faction Building**
Every faction has a unique building available to them in a game. This will usually be in line with the factions play style and general gameplan. Each faction has at least two exclusive buildings that they can choose from at the beginning of the game.

### Unit Enhancement
Unit enhancement is a mechanic that rewards forethought. You can interact with this mechanic using the unit enhancement station or enlistment tube in your base of operations, and can choose to pay to enhance an action for an amount of uses proportional to the gold cost. This encourages sending out units with a clear purpose, as you have to consider which action is best to use in that situation. Not all enhanced actions should offer a direct upgrade. Even if they are stronger, they should not fulfill the exact same purpose as the default version. Unit enhancements will generally have a tradeoff.

---

## Units 
### General Unit Properties
* **Unit Type**
* **HP**
* **Traversal / Action Token Count**
* **Can Walk Through**

### Unit Types
* **Commander**
* **Offensive**
* **Support**
* **Builder**
* **Operator**

### Actions
Typically, units will have 4 actions available to them, including the traditional movement option. All actions have a token cost in order to be used. All actions have an enhanced version that can be accessed using the unit enhancement station. Any given action can be described as one of four different types: **movement**, **attack**, **construction**, or **utility**. 

**Movement Actions**  
Accepted Tokens: Traversal & Action   
Action Stats: Movement Distance  
These actions typically have a focus on moving the unit, however this is not enforced as any action can still count as a movement action as long as it accepts traversal tokens. Typically, units cannot move onto a tile if the difference in elevation is greater than 1. 

**Attack Actions**  
Accepted Tokens: Action   
Action Stats: Damage, Range, Target Unit (Self, Other), Attack Type (Melee, Ranged)  
Any attack action aims to damage another unit's HP. Typically, if there is a difference in elevation, the range of melee attacks is reduced by that amount. Ranged attacks gain more range if the unit is at a higher elevation than the target tile, and inversely, they lose range if they are at a lower elevation.

**Construction Actions**  
Accepted Tokens: Action   
Action Stats: Allowed Count  
Construction actions are normally exclusive to builder units, and are used to construct a structure. In addition to costing action tokens, there is typically a limit on the number of structures a builder unit can create from one action.

**Utility Actions**  
Accepted Tokens: Action  
Action Stats: Range, Target Unit (Self, Other, None)  
Utility actions are for any action that does not match the description of a movement, attack, or construction action.

### Commanders
* **All General Unit Properties**
* **Commanded Unit Limit**
* **Morale (Kill Count Limit Before Loss)**

There are at least two commanders to choose from per faction. Each commander should encourage playing the faction in a different way, adding a layer of player expression.

---

## Structures
### General Structures Properties
* **HP**
* **Builder**
* **Is Traversable**
* **Is Operable**
* **Operator Conditions**
* **Operation Range**

### Operable Structures
There are some structures which do not primarily act as passive effects, but instead rely on being operated. They can be operated if a unit is next to them that matches the operator conditions of the structure. These structures will have actions available to them, similar to units.

---

## Unexplored Ideas
### Unlock More Units Throughout the Game 
Throughout the game, factions can have different conditions which when met, unlocks a specific unit for that game, which is not available by default. This encourages side goals to aim for in order to unlock a specific unit you want for a battle.

### Disengage Penalty
Attempting to flee too far from a fight incurs some sort of penalty that puts the unit at a disadvantage, for example allowing the unit they were fighting to inflict a critical strike.

### Early Game Mechanic 
A mechanic that encourages early development of the faction over immediate aggression.

### Unit Rest
Heals a minor amount at the cost of the unit's turn, this means they get to heal without having to run to the recovery hut, but the tradeoff is that they lose their turn.

### Permanent Enhancement on Enlistment 
When enlisting, you are given the option to pay an amount to permanently enhance an ability instead of enhancing it for X number of uses.

---

## Scrapped Ideas
### Environmental Interactions
Adds depth to the environment using naturally occurring entities such as trees that can be broken.

**REASON:** This idea may be revisited in the future but currently I can’t imagine it providing much in the way of player decisions, instead just being a side of the game to be ignored.

### Add Structure to Defend in Base
This will act as another win condition, where if it gets destroyed then the player loses the game. 

**REASON:** As it stands, the game does not need another win condition and that would just be adding more clutter for no reason.

### Making a Sub Base
Being able to make a sub base can add more emphasis on gaining positional advantage, as a more aggressively placed base will allow for easier access to its services. The base can break down after X turns unless it is maintained by a builder, which then gives the opposing faction a target.

**REASON:** I currently see no real downside to making a sub base hence why I am hesitant to add it. It will need to have a large risk associated with it before it is reconsidered.

# dev.Run

### *Keep your branch clean. Try not to ship the bugs.*

🏆 **Judges' Choice — Diversion Summer Jam 2025**

dev.Run is a minimalist roguelite runner developed **solo in approximately 3 days** for Diversion Summer Jam 2025.

The jam was organized by **Diversion**, a version control system for game development, and its theme was — appropriately — **"Branches"**.

I interpreted the theme quite literally: in dev.Run, the player controls a repository branch, collecting good pieces of code while avoiding bugs and other development hazards.

**Play on itch.io:** https://az7atoth.itch.io/devrun

## Gameplay

At its core, dev.Run is an endless runner with roguelite elements.

During a run, the player collects upgrades and encounters different modifiers that affect gameplay. A perk system allows individual characteristics to be improved, including survivability and other gameplay parameters.

The combination of perks, buffs, debuffs and time-manipulation mechanics means that different parts of the game may need to progress at different rates.

## Technical Highlights

Despite the short development time, the project contains several systems built specifically around its gameplay requirements:

- **Multiple independent time modifiers** — gameplay speed, transitions, perks, buffs, debuffs and individual events can operate with separate time coefficients instead of relying entirely on a single global `Time.timeScale`.
- **Reactive game state** — gameplay parameters and time modifiers use reactive properties, allowing systems to respond to state changes without requiring direct references between every component.
- **Data-driven perk system** — perks are configured through ScriptableObjects, including their effects, progression and presentation data.
- **Async transitions with UniTask** — gameplay transitions and time changes use asynchronous flows with cancellation support.
- **Object pooling** — reusable gameplay objects such as generated branches are pooled rather than repeatedly instantiated and destroyed.
- **Timeline and camera transitions** — presentation sequences are integrated into the game flow alongside asynchronous UI and gameplay transitions.

## Art & Presentation

The game's minimalist visual style was created specifically for the jam and designed entirely by me.

Despite the very short development time, I put significant effort into making the game feel polished through:

- Animation
- Visual effects
- UI and graphic design
- Camera work
- Timeline sequences
- Gameplay feedback and transitions

The deliberately minimal presentation also fit the programming/version-control theme of the jam.

## Development

The jam itself lasted longer, but I initially decided not to participate because I did not have an idea I was happy with.

After coming up with the concept, I changed my mind and built the game **solo in approximately three days**.

With such a short deadline, the priority was rapid iteration and getting the complete experience to a polished state rather than designing production-level architecture. Some systems — particularly the interaction between multiple independent time modifiers — became more complex as new mechanics were added during the jam.

The repository is preserved largely as it existed at the end of the event and reflects those development constraints.

## Result

dev.Run received the **Judges' Choice** award at Diversion Summer Jam 2025.

For me, the project is primarily an example of **rapid prototyping, working with unusual gameplay requirements, managing interconnected game systems, and delivering a polished solo project under a very strict deadline**.

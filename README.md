# Weapon System

Modular weapon system for Unity with C#. This system lets you build a wide variety of weapons with a primary attack and, optionally, a secondary one. Although the system is designed mainly for firearms, it can also be used to create melee weapons.

- Firearms are built by inheriting from `FirearmBase`, which connects all the components and applies the weapon's own logic.
- Classes that inherit from `BarrelBase` implement the firing logic.
- Classes that inherit from `WeaponLoaderBase` manage the weapon's ammunition.

In addition, logic and feedback are kept separate, which increases script reusability.

## Example weapons

For this repository I created 3 example weapons to illustrate some of the possibilities this pattern offers.

### Gun

Semi-automatic pistol that fires a raycast and damages the target. Ammo is limited.

<img width="480" height="270" alt="Gun" src="https://github.com/user-attachments/assets/2f0a4ae3-6fd2-4ea5-be79-9188eeeb7837" />

### Devil Knife

Its primary attack throws physical daggers at a constant rate. Its secondary attack throws multiple daggers forward in an arc. Ammo is unlimited, and it cannot attack until the secondary attack's cooldown has recovered.

<img width="480" height="270" alt="DevilKnife" src="https://github.com/user-attachments/assets/d30d11ca-2fce-45be-96b5-871c95ab13da" />

### Water Gun

The primary attack shoots a cone-shaped jet of water forward. The secondary attack charges a water bubble and then launches it; on impact it deals damage in a radius that grows with the size of the bubble. Ammo is limited but regenerates automatically. The secondary attack has a cooldown before it can be used again.

<img width="480" height="270" alt="WaterGun" src="https://github.com/user-attachments/assets/21385b5c-f34e-44a8-ab68-a8f2ec74c6b5" />

## Demo

The repository only includes the code for the system and the example weapons. I have also added a Unity Package with a demo that includes all the assets and a test scene.

The demo uses code from my repositories:

- [Character System](https://github.com/RoniaV/CharacterSystem)
- [Unity FSM](https://github.com/YOUR-USERNAME/Unity-FSM)
- [Unity Utils](https://github.com/YOUR-USERNAME/Unity-Utils)

The following third-party assets are also used:

- TextMeshPro, Unity
- Weapon Stylized Pack, nappin
- Cartoon FX Remaster, JMO Assets

The demo is built with Unity 2021.3.45f2 using the Built-in Render Pipeline.

## AI Disclaimer

AI assistance was used in this repository for:

- `ShapeSampler`, which was generated with AI.
- The code comments, which were written with AI assistance.
- A final cleanup pass over the code (formatting consistency and dead code removal).

The rest of the code was written by me.

# AccountInBank-GTA5
[![Build Status](https://ci.appveyor.com/api/projects/status/bm28vepxi85dmqn5/branch/master?svg=true)](https://ci.appveyor.com/project/IncoCode/accountinbank-gta5)

###**__Still WIP__**.

This script lets you store ~~stolen~~ your money in bank account and earn interest (0.1% per day by default).

Maximum balance: **2 147 483 647** for _each_ character.

How to use
----------

 - Press **B** to mark the nearest ATM on the map;
 - Press **O** when you near ATM to open ATM menu.

Requirements 
--------------

- Microsoft .NET Framework 4.5;
- Microsoft Visual C++ Redistributable Package for Visual Studio 2013 (x64);
- Script Hook V;
- Community Script Hook V .NET ( https://ci.appveyor.com/project/crosire/scripthookvdotnet%C2%A0/build/artifacts ).

How to install
--------------

Place the .dll files into your "**scripts**" folder, located in the main GTA V directory. If it doesn't exist, create it.

Known issues
--------------
Don't type numbers using your numpad. Sometimes it's works incorrect.

ATM location
--------------
If you found ATM that not exists in the list - let me know.

Changelog
--------------
v1.0.2
- Added features: LoseCashOnArrest and LoseCashOnDeath (disabled by default);
- Ability to transfer money between characters accounts;
- Bug fixes.

v1.0.1
- Ability to change default keys in .ini-file;
- Some fixes.
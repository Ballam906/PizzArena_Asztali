![.NET Version](https://img.shields.io/badge/.NET-8.0-blueviolet)
# PizzArena Admin Panel

Ez egy .NET alapú asztali (WPF) alkalmazás, amellyel a PizzArénához tartozó adatok kezelhetőek. Az alkalmazás egy elkülönített ASP.NET Core Web API-hoz csatlakozik, biztosítva a biztonságos és gyors adatkezelést.

## Főbb funkciók
* **Felügyelés:** Adatok egyszerű szerkesztése,törlése,hozzáadása, globális beállítások módosítása.
* **Felhasználókezelés:** Új adminisztrátorok regisztrálása validált adatokkal.
* **Rendeléskövetés:** Rendelés követés éttermek alapján, rendelési tételek és rendelés állapot módosítás.
* **Modern UI:** Felhasználóbarát felület, tiszta elrendezés.

# Telepítés és beállítás:

1. Telepítő fájlok elérése
  A telepítéshez szükséges setup.exe telepítőt itt találja:
    PizzArena_Asztali\setup.exe

2. Telepítés folyamata
   1. Futtassa a setup.exe fájlt a telepítő-varázsló elindításához.
   2. A varázsló lépésről lépésre vezeti végig a folyamaton:
   3. Beállíthatja a telepítési útvonalat.
   4. Megadhatja, hogy mely Windows-felhasználók érhessék el a programot.
   5. A folyamat végén egy megerősítő ablak jelzi a sikeres telepítést.

A telepítés befejezése után az alkalmazás automatikusan létrehoz egy parancsikont az asztalon, amellyel azonnal indítható az Admin Panel.

# Beállítások (konfiguráció):
Az alkalmazás rugalmassága érdekében az API elérési útvonala nincs a forráskódba égetve, így az bármikor módosítható.
Az API URL-címének módosítása:
 1. Keresse meg a telepítési könyvtárban az App.config fájlt.
 2. Nyissa meg a fájlt egy tetszőleges szövegszerkesztővel (pl. Notepad, VS Code).
 3. Keresse meg az <appSettings> szakaszt, és írja át az ApiUrl kulcs értékét:
    # <appSettings>
    #   <add key="ApiUrl" value="http://ide-ird-az-uj-api-cimet.hu/api" />
    # </appSettings>

# PizzAréna Github linkje: (https://github.com/Ballam906/PizzArena)

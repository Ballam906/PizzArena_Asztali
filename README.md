# PizzArena Admin Panel

Ez egy .NET alapú asztali (WPF) alkalmazás, amellyel a PizzArénához tartozó adatok kezelhetőek. Az alkalmazás egy elkülönített ASP.NET Core Web API-hoz csatlakozik, biztosítva a biztonságos és gyors adatkezelést.

## Főbb funkciók
* **Felügyelés:** Adatok egyszerű szerkesztése,törlése,hozzáadása, globális beállítások módosítása.
* **Felhasználókezelés:** Új adminisztrátorok regisztrálása validált adatokkal.
* **Rendeléskövetés:** Dinamikus szűrés és keresés rendelés azonosító (ID) alapján.
* **Modern UI:** Felhasználóbarát felület, tiszta elrendezés.

# Konfiguráció:
Ellenőrizd az ApiService.cs fájlban az API elérését:
**_client.BaseAddress = new Uri("https://localhost:5000/");**

![Főoldal](docs/images/fooldal.png)

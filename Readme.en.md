# Schumix2

# Statement

This code is intended to relay its predecessor. It is lay new basis and got many newness.
More faster and reliable than Schumix. The code is written in c#.

# Platforms

The bot has compatibility with the windows and linux systems. It might be run on Mac OS X but it is not tested.
Under Windows the `.NET Framework 4.0` or higher is recommended.
Under Mono at least the `2.10` or higher is needed.

# Run source/Compile

## Windows

The compiling is simple. Open the `Schumix.sln` file. Choose the configuration that fits for you and compile it.

## Linux

Open the `Schumix.sln` file. Choose the configuration that fits for you and compile it.

## Linux terminal

Install the `mono-xbuild` package or from source. After run the `build.sh` command and the code is compiled.

# Code commissioning

Navigate to the `Run` folder and in that proper folder for the configuration. Run the exe. The program is generates its config file. If there will be some problem then create a `Configs` named folder and from the root folder copy in that folder the `Schumix.yml` named file.

# Install

Csak akkor használjuk ezt az opciót ha úgy szeretnénk telepíteni a botot mint ha be akarjuk telepíteni a rendszerbe. Figyelem! Rendszergazadi jog valószinüleg szükséges lesz a telepítés végső szakaszához.

## Archlinux

Futtassuk `sh createarchlinuxpkg.sh` parancsot. Ha lefutott megjelenik egy `schumix.pkg.tar.xz` (hasonló lesz a neve) nevű fájl. Ezt telepítsük a `sudo pacman -U schumix2.pkg.tar.xz` (csomag fájl neve hasonló lesz) paranccsl és már készen is vagyunk. A botot a `schumix` paranccsal futtathatjuk.

## Debian/Ubuntu

Futtassuk `sh createdebianpkg.sh` parancsot. Ha lefutott megjelenik egy `schumix.deb` nevű fájl. Ezt telepítsük a `sudo dbkg -i schumix.deb` paranccsl és már készen is vagyunk. A botot a `schumix` paranccsal futtathatjuk.

## Windows
Navigáljunk az Installer mappába. Futtassuk a `Schumix.iss` nevű fájlt. Ha lefutott kapunk egy `Setup.exe` nevű telepíthető állományt. Futtasuk és értelemszerüen telepítsük. A többit szerintem nem kell részletezni :)

# Config settings

## Server

* **Enabled:** Its value `true` or `false`. Defines if the program connects to the server. Default: `false`
* **Host:** Server's address.
* **Port:** Server's port. Default: `35220`
* **Password:** Server's identify password.

## Irc

Yaml konfignál:
Ha több szerverre szeretnék felkapcsolódni vagy egyre többször akkor az egész irc részt (Irc: ...) le kell másolni még egyszer és ott külön be kell állítani az adatokat valamint Irc(szám) ként kell megadni. Pl: Irc2: .... (ide pedig minde úgy jön utána ahogy volt csak az Irc-nél kell átírni az újat).
Xml konfignál:
Ha több szerverre szeretnék felkapcsolódni vagy egyre többször akkor az egész irc részt (`<Irc> ... </Irc>`) le kell másolni még egyszer és ott külön be kell állítani az adatokat.
* **ServerName:** A szerver neve. Ezzel lehet beállítani hogy többszerveres módban hogy mi legyen az egyes szervereket megkülönböztető név. FIGYELEM: Nem egyezhet meg a többi szerver nevével (kis és nagybetüt nem különbőzteti meg)!
* **Server:** Ide kell beírni a szerver nevét ahova csatlakozni szeretnénk.
* **Port:** A szerver portja. Alapértelmezés: `6667`
* **Ssl:** Értéke `true` vagy `false` lehet. Ezzel aktiválható a kapcsolódás olyan irc szerverre ahol ssl protokol van használva. Alapértelmezés: `false`
* **NickName:** Elsõdleges név.
* **NickName2:** Másodlagos név.
* **NickName3:** Harmadlagos név.
* **UserName:** Felhasználó név.
* **UserInfo:** Információ a felhasználóról.
* **MasterChannel:**
    * **Name:** Elsõdleges csatorna ahova csatlakozik minden esetben a bot. Ennek a neve itt változtatható meg. Az adatbázisból nem törölhetõ. Yaml konfig esetén "" jelek közé kell rakni a csatornát. Pl: Name: "#schumix2"
    * **Password:** Az elsődleges csatornához tartozó jelszó.
                    Alapértelmezés: (semmi)[Ez azt jelenti hogy nem add meg jelszót az elsődleges csatornához.]
* **IgnoreChannels:** Letilthatók a nem kívánatos csatornák vele. Ami itt szerepel oda nem megy fel a bot. Ezen rész letiltja a bot rendszerében szereplõket is.
                      Tehát ha abból nem akarunk valahova felmenni akkor is használhatjuk ezt törlés helyett. Vesszõvel elválasztva kell egymás útán írni öket.
                      `pl: #teszt,#teszt2 vagy szimplán #teszt`
* **IgnoreNames:** Letilthatóak vele a nem kívánatos személyek. Így csak az használhatja a botot aki megérdemli.
                   `pl: schumix,schumix2 vagy szimplán schumix`
* **NickServ:**
    * **Enabled:** Értéke `true` vagy `false` lehet. Ezen rész határozza meg hogy a nickhez tartozó jelszó el legyen-e küldve. true = igen, false = nem.
                   Alapértelmezés: false
    * **Password:** Nickhez tartozó jelszó.
* **HostServ:**
    * **Enabled:** Értéke `true` vagy `false` lehet. Ezen rész határózza meg hogy ha van a nickhez vhost akkor bekapcsolodjon-e. Alapértelmezés: `false`
                   Mert ha nincs akkor megjelenitödhet az ip ezért olyankor ajánlott false értékre tenni.
    * **Vhost:** Értéke `true` vagy `false` lehet. Ezen rész határózza meg hogy a nickhez tartotó vhost aktiválásra kerüljön-e. Alapértelmezés: `false`
* **Wait:**
    * **MessageSending:** Üzenet küldésének késleltetése. Legföbbként flood ellen van.
* **Command:**
    * **Prefix:** A parancsok elõjele. Yaml konfig esetén "" jelek közé kell rakni a parancsot. Pl: Prefix: "$". Alapértelmezés: `$` (Fõ parancs xbot. Ezzel a parancselõjelel így néz ki: `$xbot`)
* **MessageType:** Értéke `Privmsg` vagy `Notice` lehet. Meghatározza hogy milyen formában küldje az üzeneteket a szerver felé. Alapértelmezés: `Privmsg`

## Log

* **FileName:** Meghatározza hova mentődjenek el a log információk. Alapértelmezés: `Schumix.log`
* **DateFileName:** Ha ez a beállítás bekapcsolásra került akkor a log fájl nevéből létrehoz egy mappát a program és abba az indítás dátumával menti el a logot. Így áttekinthetőbbé válik.
                    Alapértelmezés: `False`
* **MaxFileSize:** Meghatározza a log fájlt maximális méretét. Ha eléri azt a fájl akkor törlődik és a program csinál helyette egy újat.
                    Alapértelmezés: `100` (mb-ban értendő)
* **LogLevel:** Meghatározza hogy a konzolba milyen üzenetek kerülnek kiírásra. Alapértelmezés: `2`
    * **Szintjei:** `0` (Normális üzenetek és a sikeresek)
                    `1` (Figyelmeztetések)
                    `2` (Hibák)
                    `3` (Hibakeresõ üzenetek)
* **LogDirectory:** A log üzenetek mentése abba a mappába ami megvan adva. Alapértelmezés: `Logs`
* **IrcLogDirectory:** Az irc csatornák és egyéb üzenetének mentése abba a mappába ami megvan adva. Alapértelmezés: `Csatornak`
* **IrcLog:** Értéke `true` vagy `false` lehet. Meghatározza hogy a konzolba kiirásra kerülhetnek-e az irc-tõl jövõ üzenetek. Alapértelmezés: `false`

## MySql

* **Enabled:** Értéke `true` vagy `false` lehet. Meghatározza hogy mysql alapú-e az adatbázis. Alapértelmezés: `false`
* **Host:** A mysql szerver címe.
* **User:** A szerver felhasználó neve.
* **Password:** A szerver jelszava.
* **Database:** Az adatbázis amiben megtalálhatók a bothoz tartozó táblák.
* **Charset:** Az adatbázisba menõ adatok kódolását és olvasását határozza meg.
               Alapértelmezés: `utf8`

## SQLite

* **Enabled:** Értéke `true` vagy `false` lehet. Meghatározza hogy sqlite alapú-e az adatbázis. Alapértelmezés: `false`
* **FileName:** Az sqlite fájl neve.

## Addons

* **Enabled:** Értéke `true` vagy `false` lehet. Engedélyezi az addonok betöltését. Alapértelmezés: `true`
* **Ignore:** Azon addonok melyeket nem szeretnénk inditáskor betölteni. Vesszõvel elválasztva kell egymás útán írni öket. `(pl: TesztAddon,Teszt2Addon vagy szimplán TesztAddon)`
              Alapértelmezés: `MantisBTRssAddon,SvnRssAddon,GitRssAddon,HgRssAddon,WordPressRssAddon,TesztAddon`
* **Directory:** Az addonok mappája ahol tárolva vannak és ahonnét betöltésre kerülnek. Alapértelmezés: `Addons`

## Scripts

* **Lua:** Értéke `true` vagy `false` lehet. Engedélyezi a lua fájlok betöltését. Alapértelmezés: `false`
* **Python:** Értéke `true` vagy `false` lehet. Engedélyezi a python fájlok betöltését. Alapértelmezés: `false`
* **Directory:** A script-ek mappája ahol tárolva vannak és ahonnét betöltésre kerülnek. Alapértelmezés: `Scripts`

## Crash

* **Directory:** Meghatározza az összeomláskor keletkező mappa nevét. Alapértelmezés: `Dumps`

## Localization

* **Locale:** Meghatározza hogy a kód milyen nyelven fusson. (csak az irc és konzol parancsokra vonatkozik)
              Alapértelmezés: `enUS`

## Update

* **Enabled:** Értéke `true` vagy `false` lehet. Engedélyezi az automatikus frissítést. Alapértelmezés: `false`
* **Version:** Meghatározza melyik verzióra szeretnénk frissíteni. Current vagy stable lehet. A current az utolsó verzó ami a tárolóban van a stable pedig az utolsó stabil verzió.
               Alapértelmezés: `stable`
* **Branch:** Beállítható vele az ág (branch). Ez csak a current verziók esetében érdekes. Alapértelmezés: `master`
* **WebPage:** A megadott weboldalcímről tölti le a frissítéseket. Alapértelmezés: `https://github.com/megax/Schumix2`

## Shutdown

* **MaxMemory:** Meghatározza a program leállítását ha eléri a megadott memória nagyságot. Ha több szerverre is csatlakozik a bot akkor annyival fog tovább nőni ez a korlát ahány irc szerver be van állítva a konfigba.
                 Alapértelmezés: `100` (mb-ban értendő)

## Flooding

* **Seconds:** Meghatározza mennyi időnként fusson le a flood elemzése. Alapértelmezés: `4` (másodpercben)
* **NumberOfCommands:** Meghatározza hányszor használhatja a parancsot adott személy a megadott indőn belül. Ha többet add meg akkor egy percre letiltja a program a parancsainak használatát annak a személynek. Alapértelmezés: `2`

## Clean

* **Config:** Értéke `true` vagy `false` lehet. Engedélyezi a konfig mappában a régi fájlok takarítását/törlését. Alapértelmezés: `false`
* **Database:** Értéke `true` vagy `false` lehet. Engedélyezi az adatbázis takarítását. Alapértelmezés: `false`

# Adatbázis beüzemelése

## MySql

Ha a jó öreg mysql alapú adatbázist szeretnénk használni állítsuk a konfig fájlban `(lásd: <MySql><Enabled>false</Enabled>)` az engedélyét true értékre.
Ezután az `Sql` mappából töltsük fel az adatbázisunkat. Ha bármiféle javítás jön a kódhoz vagy újítás nem kell az agész adatbázist újra töltenünk.
Csak az `Updates` mappából frisistsük a megfelelõ verzió szám alapján.

## SQLite

Ha az SQLite alapú adatbázist szeretnénk használni állítsuk a konfig fájlban `(lásd: <SQLite><Enabled>false</Enabled>)` az engedélyét `true` értékre.
Majd másoljuk az `Sql` mappából a `Schumix.db3` fájlt az exe mellé. Ezen fájl neve megváltoztatható de akkor a konfig fájlban is meg kell vátoztatni.
Természetesen az elérési út is a névvel együtt.

# Figyelmeztetés!

**Csak egy adatbázis lehet aktiv. Olyan nem lehet hogy egyse vagy kettõ. Ezekben az esetekben a kód leáll és nem fut tovább.**

Ha mind ezekkel megvagyunk már csak inditanuk kell és használni a kódot :)

# Apróságok

* Az álltalam készített addonokhoz tartoznak álltalában konfig fájlok. Ezek is szintén a `Configs` mappába generálodnak és ott állíthatóak be.
* A kódban továbbá elhelyezésre került egy bot parancs is. Ez a parancs az elsõdleges névbõl tevõdik össze. Példa rá: `schumix2, help`
  Fontos a parancs szerkezete `<elsõdleges nick>, parancs`
* A kódhoz ki és be kapcsolható funkciók tartoznak. Ezt csak is gizárólag admin vezérelheti.
  Funkciók használatáról leírás: `$help function` (természetesen az az elõjel kell ami megadásra került a konfigban)
* Ha már megemlítésre került. Az admin hozzáadása konzolból történik elõször. `admin add <admin neve>`
  Majd amit kapunk jelszót privát üzenetben el kell küldeni a botnak a jelszót ezen módon: `$admin access <jelszó>`
  Ha másikat szeretnénk akkor: `$admin newpassword <régi> <új>`
* És végül a konzol parancsok. Ha már megemlítettem ;) Szóval a lista a help parancsal kapható meg.
  Többit ki kell tapasztalni mert egyenlõre nincs hozzá help.
* Bármi lemaradt volna tudok segítséget nyújtani az irc.rizon.net szerveren a `#schumix, #schumix2` vagy `#hun_bot` csatornán.
* Yaml konfignál minden olyan adatot amely különleges karaktert tartalmaz (pl: #) azt idézőjelek közé kell helyezni "" mert a program máskülönben megpróbálná értelmezni és az hibát okozna.
* Remélem meg fog tetszeni a bot :)
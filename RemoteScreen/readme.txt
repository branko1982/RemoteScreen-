Táto aplikácia má 3 časti.
RemoteScreenClient -> časť ktorá beží na zariadení ktorého pracovnú plocha sa priemieta
RemoteScreenOperatorTool -> nástroj kde je možné vybrať vziadlenú obrazovku a sledovať jej výstup
RemtoeScreenServer -> kľúčová časť cez ktorú prúdia dáta od klientov k operátorovy


Najprv som uvažoval že by to fungovalo čisto na dvoch aplikáciach, klient a server, ale je to nepraktické.

V tomto štádiu klient viac menej funguje.
Len tak zo srandy som otestoval či klient bude premietať aj taký obsach hry ako... povedzme...
mount & blade, a dokázal to. 

Snímka zo zariadenia klienta, ktorá sa prenáša cez sieť má veľkosť asi 140 kilobajtov.
Urobil som nejaký základ aby bol snímok čo najmenší. To aký je veľká snímka obrazovky záleží od toho čo
si užívateľ pozerá na ploche. Čím menej zložitý obraz snímka obrazovky obsahuje, tým bude menšia.


Uvažoval som dlho či použiť python alebo C#. Pre serverovú časť.
Nakoniec som použil C#, pretože programovanie v ňom mi príde intuitívnejšie a pohodlnejšie.
Tiež, celý projekt bol v C#, takže aj serverová časť, mohla byť kľudne urobená v C#. Proste som usúdil že to je lepšie,
python má divné konvencie pomenovávania premenných, ktoré sa mi moc nepáčia, nemá modifikátory prístupu ako také,
a pri práci v C# oddelím kód aplikácie od reálneho skompilovaného produktu.
S mono runtime C# aplikácia pobeží aj na linuxe. Žiadny problém.



RemoteScreenClient:
    spustí sa, (ip adresa a port na ktorý sa klient má pripojiť sa klientovy predajú cez príkazový riadok )
    - pripojí sa na server s tým že mu odošle string zo základnými informáciami o sebe
    - serveru pošle nejaké základné informácie ako názov zariadenia, rozlíšenie.
    







/* =======================================================
   Externe Publieke API: Weerbericht (Weerlive.nl)
   ======================================================= */

const WEER_API_KEY = "6e8e9f4515";

// Wacht tot de hele DOM is opgebouwd voordat we elementen selecteren
document.addEventListener("DOMContentLoaded", () => {
    const weerCard = document.getElementById("weer-card");
    const stadInput = document.getElementById("weer-stad-input");
    const zoekKnop = document.getElementById("weer-zoek-btn");

    // Veiligheidscheck: check of de HTML elementen daadwerkelijk bestaan
    if (!weerCard || !stadInput || !zoekKnop) {
        console.error("Een of meerdere HTML-elementen (#weer-card, #weer-stad-input, #weer-zoek-btn) ontbreken in de pagina!");
        return;
    }

    // [x] Laadstatus tonen
    const toonWeerLaden = () => {
        weerCard.innerHTML = "";
        const laadTekst = document.createElement("p");
        laadTekst.classList.add("weer-status", "loading");
        laadTekst.textContent = "Weerbericht ophalen...";
        weerCard.appendChild(laadTekst);
    };

    // [x] Foutstatus netjes afhandelen en tonen
    const toonWeerFout = (bericht) => {
        weerCard.innerHTML = "";
        const foutBox = document.createElement("div");
        foutBox.classList.add("weer-status", "error");

        const titel = document.createElement("strong");
        titel.textContent = "Fout bij ophalen van het weer: ";

        const foutTekst = document.createElement("span");
        foutTekst.textContent = bericht;

        foutBox.append(titel, foutTekst);
        weerCard.appendChild(foutBox);
    };

    // [x] Opgehaalde data dynamisch renderen met DOM-methoden
    const renderWeerData = (data) => {
        console.log("Data wordt gerenderd:", data);
        weerCard.innerHTML = ""; // Container leegmaken

        const weerInfo = data.liveweer[0];

        // 1. Weer-icoon
        const icoon = document.createElement("img");
        // Weerlive iconen op hun server:
        icoon.src = `https://weerlive.nl/images/${weerInfo.image}.png`;
        icoon.alt = weerInfo.samenv || "Weer status";
        icoon.classList.add("weer-icoon");

        // Voorkom layout-fout als de externe afbeelding niet wil laden
        icoon.onerror = () => {
            icoon.style.display = "none";
        };

        // 2. Info container
        const infoDiv = document.createElement("div");
        infoDiv.classList.add("weer-info");

        const stadTitel = document.createElement("h3");
        stadTitel.textContent = `${weerInfo.plaats}`;

        const temperatuur = document.createElement("p");
        temperatuur.classList.add("weer-temp");
        temperatuur.textContent = `${weerInfo.temp}°C`;

        const omschrijving = document.createElement("p");
        omschrijving.classList.add("weer-desc");
        omschrijving.textContent = weerInfo.samenv;

        // 3. Extra weersdetails
        const details = document.createElement("div");
        details.classList.add("weer-details");

        const wind = document.createElement("span");
        wind.textContent = `Wind: ${weerInfo.windkmh} km/u (${weerInfo.windr})`;

        const vochtigheid = document.createElement("span");
        vochtigheid.textContent = `Luchtvochtigheid: ${weerInfo.lv}%`;

        const gevoel = document.createElement("span");
        gevoel.textContent = `Gevoel: ${weerInfo.gtemp}°C`;

        details.append(wind, vochtigheid, gevoel);

        // Alles toevoegen aan de DOM
        infoDiv.append(stadTitel, temperatuur, omschrijving, details);
        weerCard.append(icoon, infoDiv);
    };

    // [x] Externe JSON-data ophalen met behulp van Fetch API
    const haalWeerOp = async (stad) => {
        toonWeerLaden();

        const url = `https://weerlive.nl/api/weerlive_api_v2.php?key=${WEER_API_KEY}&locatie=${encodeURIComponent(stad)}`;

        try {
            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`Serverfout (status: ${response.status})`);
            }

            const data = await response.json();

            if (!data.liveweer || data.liveweer.length === 0) {
                throw new Error("Geen weergegevens ontvangen.");
            }

            renderWeerData(data);

        } catch (fout) {
            console.error("Weer API fout:", fout);
            toonWeerFout(fout.message || "Er ging iets mis met het ophalen van het weer.");
        }
    };

    // [x] Event listeners koppelen
    zoekKnop.addEventListener("click", () => {
        const stad = stadInput.value.trim();
        if (stad) haalWeerOp(stad);
    });

    stadInput.addEventListener("keypress", (e) => {
        if (e.key === "Enter") {
            const stad = stadInput.value.trim();
            if (stad) haalWeerOp(stad);
        }
    });

    // Eerste keer ophalen
    haalWeerOp("Den Haag");
});
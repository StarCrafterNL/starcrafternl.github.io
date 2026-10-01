// [x] Projectendata definiëren in een JavaScript array
const projecten = [
    {
        id: "autstede-demo",
        titel: "Formulier naar pdf Demo",
        beschrijving: "Dit is een demo van een formulier naar pdf webapp die ik heb gemaakt voor Autstede. Deze demo laat zien dat de gegevens van het formulier automatisch in een pdf opgeslagen kunnen worden.",
        githubUrl: "https://github.com/StarCrafterNL/autstede-project",
        afbeelding: "../images/overmij/autstede.png",
        categorie: "Individueel",
        datum: "2026-09-28"
    },

    {
        id: "avond4daagse",
        titel: "Webapplicatie Avond4daagse",
        beschrijving: "Op het moment mee aan de slag. Dit is casus die ik gekregen heb om in groepsverband te werken en de leeruitkomste te behalen van het derde semester.",
        githubUrl: "https://github.com/StarCrafterNL/Avond4-DaagseGroep3",
        afbeelding: "../images/portfolio/avond 4 daagse.png",
        categorie: "Groepsproject",
        datum: "2026-09-02"
    },
    {
        id: "webportfolio",
        titel: "WebPortfolio",
        beschrijving: "Dit is mijn webportfolio, waarin ik mijn projecten en vaardigheden presenteer. De website is gebouwd met HTML en CSS dit zal later worden aangevuld met JavaScript. Deze website is nog in ontwikkeling en is de vorm waarmee ik de leeruitkomst van WPFW zal aantonen.",
        githubUrl: "https://github.com/StarCrafterNL/starcrafternl.github.io",
        afbeelding: "../images/portfolio/webportfolio.png",
        categorie: "Individueel",
        datum: "2026-09-01"
    },
    {
        id: "veilingklok",
        titel: "Digitale veilingklok RoyalFlora",
        beschrijving: "Voor de eerste keer dat ik aan het 3de semester begon heb ik samen met mijn projectgroep een casus gekregen om een digitale veiling te maken voor RoyalFlora. Hierbij is het grootste onderdeel waar ik aan heb gewerkt de veiling pagina met de Veilingklok. Hierin heb ik de veilingklok gemaakt die de tijd van de veiling laat zien en de tijd die over is voor het bieden.",
        githubUrl: "https://github.com/Smeckle-git/RoyalFloraHolland",
        afbeelding: "../images/portfolio/Royal-FloraHolland.png",
        categorie: "Groepsproject",
        datum: "2023-11-10"
    }
];


const projectenContainer = document.getElementById("projecten-container");
const filterKnoppen = document.querySelectorAll(".filter-btn");
const sorteerSelect = document.getElementById("sorteer-select");


let actieveCategorie = "alles";


const renderProjecten = (projectenLijst) => {
    projectenContainer.innerHTML = "";

    projectenLijst.forEach((project) => {
        // Project wrapper met algemene class EN specifieke id-class
        const projectKaart = document.createElement("div");
        // We voegen '.portfolio-project' toe voor de algemene styling
        projectKaart.classList.add("portfolio-project", project.id);

        // Tekst container
        const tekstDiv = document.createElement("div");
        tekstDiv.classList.add("tekst");

        // Titel
        const h2 = document.createElement("h2");
        h2.textContent = project.titel;

        // Beschrijving
        const p = document.createElement("p");
        p.textContent = project.beschrijving;

        const br = document.createElement("br");

        // GitHub link
        const link = document.createElement("a");
        link.href = project.githubUrl;
        link.target = "_blank";
        link.rel = "noopener noreferrer";
        link.textContent = "Bekijk de code op GitHub";

        tekstDiv.append(h2, p, br, link);

        // Afbeelding
        const afbeelding = document.createElement("img");
        afbeelding.src = project.afbeelding;
        afbeelding.alt = project.titel;

        // Kaart samenstellen en injecteren in container
        projectKaart.append(tekstDiv, afbeelding);
        projectenContainer.appendChild(projectKaart);
    });
};


const filterEnSorteer = () => {
    // 1. Filteren
    let resultaat = [...projecten];
    if (actieveCategorie !== "alles") {
        resultaat = resultaat.filter((item) => item.categorie === actieveCategorie);
    }

    // 2. Sorteren
    const sorteerKeuze = sorteerSelect.value;
    if (sorteerKeuze === "nieuwste") {
        resultaat.sort((a, b) => new Date(b.datum) - new Date(a.datum));
    } else if (sorteerKeuze === "oudste") {
        resultaat.sort((a, b) => new Date(a.datum) - new Date(b.datum));
    } else if (sorteerKeuze === "titel-az") {
        resultaat.sort((a, b) => a.titel.localeCompare(b.titel));
    }

    // 3. Renderen
    renderProjecten(resultaat);
};

filterKnoppen.forEach((knop) => {
    knop.addEventListener("click", () => {
        filterKnoppen.forEach((btn) => btn.classList.remove("active"));
        knop.classList.add("active");

        actieveCategorie = knop.dataset.categorie;
        filterEnSorteer();
    });
});

sorteerSelect.addEventListener("change", () => {
    filterEnSorteer();
});


filterEnSorteer();
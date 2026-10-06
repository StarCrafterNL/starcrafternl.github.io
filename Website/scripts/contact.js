document.addEventListener("DOMContentLoaded", () => {
  const formulier = document.getElementById("contactformulier");
  const naamVeld = document.getElementById("naam-form");
  const emailVeld = document.getElementById("email-form");
  const berichtVeld = document.getElementById("bericht-form");
  const meldingVak = document.getElementById("form-melding");

  // Regex om een basis e-mailformaat te verifiëren
  const emailRegExp = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  // Hulpfunctie om fouten per specifiek veld te beheren en ARIA-attributen bij te werken
  const valideerVeld = (veld, foutElementId, foutbericht) => {
    let foutElement = document.getElementById(foutElementId);

    // Maak het foutelement dynamisch aan als het nog niet in de HTML staat
    if (!foutElement) {
      foutElement = document.createElement("span");
      foutElement.id = foutElementId;
      foutElement.className = "veld-fout";
      foutElement.setAttribute("aria-live", "polite");
      veld.insertAdjacentElement("afterend", foutElement);
    }

    if (foutbericht) {
      // Ongeldig: koppel aria en toon specifieke foutmelding
      veld.setAttribute("aria-invalid", "true");
      veld.setAttribute("aria-describedby", foutElementId);
      veld.classList.add("input-ongeldig");
      foutElement.textContent = foutbericht;
      return false;
    } else {
      // Geldig: herstel status
      veld.removeAttribute("aria-invalid");
      veld.removeAttribute("aria-describedby");
      veld.classList.remove("input-ongeldig");
      foutElement.textContent = "";
      return true;
    }
  };

  formulier.addEventListener("submit", (event) => {
    event.preventDefault();

    // Verwijder eventuele witruimtes aan het begin/eind
    const naam = naamVeld.value.trim();
    const email = emailVeld.value.trim();
    const bericht = berichtVeld.value.trim();

    // 1. Validatie: Naam
    let naamFout = "";
    if (naam === "") {
      naamFout = "Vul alsjeblieft je naam in.";
    } else if (naam.length < 2) {
      naamFout = "De naam moet minstens 2 tekens lang zijn.";
    }
    const isNaamGeldig = valideerVeld(naamVeld, "naam-fout", naamFout);

    // 2. Validatie: E-mail
    let emailFout = "";
    if (email === "") {
      emailFout = "Vul alsjeblieft een e-mailadres in.";
    } else if (!emailRegExp.test(email)) {
      emailFout = "Vul een geldig e-mailadres in (bijv. naam@domein.nl).";
    }
    const isEmailGeldig = valideerVeld(emailVeld, "email-fout", emailFout);

    // 3. Validatie: Bericht
    let berichtFout = "";
    if (bericht === "") {
      berichtFout = "Het bericht mag niet leeg zijn.";
    } else if (bericht.length < 10) {
      berichtFout = "Het bericht moet minimaal 10 tekens bevatten.";
    }
    const isBerichtGeldig = valideerVeld(berichtVeld, "bericht-fout", berichtFout);

    // Resultaat en feedback tonen
    if (!isNaamGeldig || !isEmailGeldig || !isBerichtGeldig) {
      meldingVak.className = "melding fout";
      meldingVak.textContent = "Controleer de gemarkeerde velden hierboven.";
    } else {
      // Alles klopt: toon succesbericht en reset het formulier
      meldingVak.className = "melding succes";
      meldingVak.textContent = "Bedankt! Je bericht is succesvol verstuurd.";
      
      // Velden netjes leegmaken
      formulier.reset();
    }
  });
});
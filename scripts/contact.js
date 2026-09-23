document.addEventListener("DOMContentLoaded", () => {
  const formulier = document.getElementById("contactformulier");
  const naamVeld = document.getElementById("naam-form");
  const emailVeld = document.getElementById("email-form");
  const berichtVeld = document.getElementById("bericht-form");
  const meldingVak = document.getElementById("form-melding");

  // Regex om een basis e-mailformaat te verifiëren
  const emailRegExp = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  formulier.addEventListener("submit", (event) => {
    event.preventDefault();

    // Verwijder eventuele witruimtes aan het begin/eind
    const naam = naamVeld.value.trim();
    const email = emailVeld.value.trim();
    const bericht = berichtVeld.value.trim();

    // Fouten array bijhouden
    const fouten = [];

    // Validatie: Naam
    if (naam === "") {
      fouten.push("Vul alsjeblieft je naam in.");
    } else if (naam.length < 2) {
      fouten.push("De naam moet minstens 2 tekens lang zijn.");
    }

    // Validatie: E-mail
    if (email === "") {
      fouten.push("Vul alsjeblieft een e-mailadres in.");
    } else if (!emailRegExp.test(email)) {
      fouten.push("Vul een geldig e-mailadres in (bijv. naam@domein.nl).");
    }

    // Validatie: Bericht
    if (bericht === "") {
      fouten.push("Het bericht mag niet leeg zijn.");
    } else if (bericht.length < 10) {
      fouten.push("Het bericht moet minimaal 10 tekens bevatten.");
    }

    // Resultaat tonen
    if (fouten.length > 0) {
      // Toon foutmeldingen
      meldingVak.className = "melding fout";
      meldingVak.innerHTML = fouten.join("<br>");
    } else {
      // Alles klopt: toon succesbericht en maak velden leeg
      meldingVak.className = "melding succes";
      meldingVak.textContent = "Bedankt! Je bericht is succesvol verstuurd.";
      


      // Hier kun je optioneel data versturen via bijv. fetch():
      // verstuurData({ naam, email, bericht });
    }
  });
});
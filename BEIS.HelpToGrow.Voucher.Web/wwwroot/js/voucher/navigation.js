const backLinks = document.getElementsByClassName("govuk-back-link");

if (backLinks && backLinks.length && window && window.history) {
  backLinks[0].addEventListener("click", () => window.history.back());
}
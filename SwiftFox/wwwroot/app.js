import { h } from 'sinuous';

function rsc(ev) {
    document.removeEventListener("readystatechange", rsc);
    document.querySelectorAll("[swiftfox-app]").forEach(element => {
        const app = element.getAttribute("swiftfox-app");
        const json = element.getAttribute("swiftfox-app-props");
        const props = JSON.parse(json);

        import(app).then(module => element.append(h(module.App(props))));
    });
}

document.addEventListener("readystatechange", rsc);
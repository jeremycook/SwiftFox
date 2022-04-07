import { o, html } from 'sinuous';
import { map } from 'sinuous/map';

function openColumnOptions(ev) {
    console.log(ev);
    console.log(this);
}

export function App(props) {

    const columnNames = o([]);
    const records = o([]);

    // See /swagger/index.html for more info
    fetch("/api/data/query?" + new URLSearchParams({
        query: JSON.stringify({
            tableSchema: props.tableSchema,
            tableName: props.tableName
        })
    })).then(response => response.json().then(data => {
        columnNames(data.columnNames)
        records(data.records)
    }));

    return html`
<table class="table table-sm table-bordered">
    <thead>
        <tr>
            ${map(columnNames, name => html`
            <th><button onclick=${openColumnOptions.bind({ name })} class="btn p-0 w-100" title="Sort and filter column">
                ${name}
                <img src="/_content/swiftfox/bootstrap-icons/icons/sliders.svg" alt="Column settings" width="24" height="24" align="right"/>
            </button></th>`)}
        </tr>
    </thead>
    <tbody>
        ${map(records, record => html`
        <tr>${record.map(value => html`
            <td>${value}</td>`)}
        </tr>`)}
    </tbody>
</table>`;
}

import { o, html } from 'sinuous';
import { map } from 'sinuous/map';

export function App(props) {

    const columnNames = o([]);
    const records = o([]);

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
            ${map(columnNames, name => html`<th>${name}</th>`)}
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

import { o } from 'sinuous';

export class ColumnCondition {
    columnName;
    /** @type {import('sinuous/observable').Observable<string>} */
    operator;
    /** @type {import('sinuous/observable').Observable<string[]>} */
    values;

    /**
     * @param {{columnName:string,operator:string,values:string[]}}
     */
    constructor({
        columnName,
        operator,
        values
    }) {
        this.columnName = columnName;
        this.operator = o(operator);
        this.values = o((values || []).map(value => value));
    }
}

export class OrderByPart {
    /** @type {string} */
    columnName;
    /** @type {string} */
    sortDirection;
}

export class TableQuery {
    /** @type {string} */
    #tableSchema;
    /** @type {string} */
    #tableName;
    #columns = o([])



    /**
     * @param {{
     * tableSchema:string,
     * tableName:string,
     * columns: string[],
     * conditions:ColumnCondition[],
     * orderBy,
     * skip,
     * take,
     * verbose
     * }}
     */
    constructor({
        tableSchema,
        tableName,
        columns,
        columnConditions,
        orderBy,
        skip,
        take,
        verbose
    }) {
        if (typeof tableSchema != "string")
            throw "The 'tableSchema' is required.";
        if (typeof tableName != "string")
            throw "The 'tableName' is required.";

        this.#tableSchema = tableSchema;
        this.#tableName = tableName;
        if (columns && columns.map) {
            this.#columns(columns.map(col => col));
        }
        if (columnConditions && columnConditions.map) {
            this.#columnConditions(columnConditions.map(col => new ColumnCondition(col)));
        }
    }

    get tableSchema() { return this.#tableSchema; }

    get tableName() { return this.#tableName; }

    /** @type {import('sinuous/observable').Observable<string[]>} */
    get columns() { return this.#columns; }
}

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

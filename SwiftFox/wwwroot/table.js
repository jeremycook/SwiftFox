import { o, html } from 'sinuous';

const data = {
    "ex.Ticket": {
        "columns": ["TicketId", "StatusId", "Subject"],
        "records": [
            ["1", "Open", "Fix this stuff"],
        ]
    },
    "ex.TicketAssignee": {
        "columns": ["TicketId", "UserId"],
        "records": [
            ["1", "2"],
        ]
    },
    "ex.User": {
        "columns": ["UserId", "Username"],
        "records": [
            ["2", "jeremy"],
        ]
    },
    "ex.TicketComment": {
        "columns": ["TicketCommentId", "TicketId", "AuthorId", "Body"],
        "records": [
            ["3", "1", "2", "Um, could you be more specific?"],
        ]
    }
};

const meta = {
    "ex.Ticket": {
        "tableSchema": "ex",
        "schemaName": "Ticket"
    }
};

export function Table(props) {
    return html`
        <li>Hello ${props.name}</li>
      `;
}

/**
 * 
 * @param {{name:string}} props
 */
export function Welcome(props) {
    return html`
        <li>Hello ${props.name}</li>
      `;
}

export function App() {
    return html`
        <ul>
          <${Welcome} name="Sara" />
          <${Welcome} name="Bob" />
          <${Welcome} name="Edite" />
        </ul>
      `;
}

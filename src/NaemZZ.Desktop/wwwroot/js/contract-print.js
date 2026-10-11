export function printHtml(html) {
    const frame = document.createElement('iframe');

    frame.style.cssText =
        'position:fixed;width:0;height:0;border:0;opacity:0;';

    document.body.appendChild(frame);

    const doc = frame.contentDocument;

    doc.open();
    doc.write(`<!DOCTYPE html>
<html lang="bg">
<head>
    <meta charset="utf-8">
    <title></title>

    <style>
        @page {
            size: A4;
            margin: 15mm;
        }

        html, body {
            margin: 0;
            padding: 0;
            background: white;
        }

        body {
            font-family: "Book Antiqua", "Times New Roman", serif;
            font-size: 11pt;
            line-height: 1.25;
            color: #000;
        }

        p {
            margin: 0 0 10px;
            min-height: 1.25em;
        }

        /* Preserve empty paragraphs from TinyMCE */
        p:empty,
        p:has(> br:only-child),
        p:has(> span:only-child > br:only-child) {
            min-height: 1.25em;
        }

        /* Preserve paragraphs containing non-breaking spaces */
        p:has(> br) {
            min-height: 1.25em;
        }

        p.contract-property {
            margin: 0 0 8px;
            line-height: 1.3;
            text-align: justify;
        }

        h1, h2, h3 {
            break-after: avoid;
        }

        table {
            border-collapse: collapse;
        }

        .mce-pagebreak {
            break-before: page;
        }

        @media print {
            html, body {
                background: white;
            }
        }
    </style>
</head>

<body>
    ${html}
</body>
</html>`);

    doc.close();

    // Convert empty TinyMCE paragraphs into explicit blank lines.
    // This preserves them even when the editor uses <p></p>.
    const paragraphs = doc.body.querySelectorAll('p');

    paragraphs.forEach(p => {
        const text = p.textContent.replace(/\u00a0/g, '').trim();

        if (!text && !p.querySelector('img, table, svg, iframe')) {
            p.replaceChildren(doc.createElement('br'));
        }
    });

    let printed = false;

    const printOnce = () => {
        if (printed) return;
        printed = true;

        frame.contentWindow.focus();
        frame.contentWindow.print();
    };

    frame.onload = printOnce;

    setTimeout(printOnce, 400);

    setTimeout(() => frame.remove(), 120000);
}
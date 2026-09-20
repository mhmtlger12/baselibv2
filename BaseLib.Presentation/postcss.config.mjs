import tailwindcss from '@tailwindcss/postcss';

const compatibility = {
    postcssPlugin: 'baselib-css-compatibility',
    OnceExit(root) {
        root.walkRules(rule => {
            const hasBlockDisplay = rule.nodes.some(node =>
                node.type === 'decl' && node.prop === 'display' && node.value === 'block');

            if (hasBlockDisplay) {
                rule.walkDecls('vertical-align', declaration => {
                    // Keep the alignment default for elements later switched to inline display.
                    // A separate rule avoids pairing vertical-align with display: block.
                    rule.cloneAfter({ nodes: [declaration.clone()] });
                    declaration.remove();
                });
            }

            rule.walkDecls('-webkit-line-clamp', declaration => {
                const hasStandardProperty = rule.nodes.some(node =>
                    node.type === 'decl' && node.prop === 'line-clamp');
                if (!hasStandardProperty) declaration.cloneAfter({ prop: 'line-clamp' });
            });
        });
    },
};

export default ({ env }) => ({
    plugins: [
        tailwindcss({ optimize: env === 'production' }),
        compatibility,
    ],
});

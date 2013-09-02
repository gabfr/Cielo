using System;
using System.Globalization;
using Awesomely.Extensions;
using Cielo.Configuration;
using Cielo.Extensions;
using Cielo.Requests.Entities;
using DynamicBuilder;

namespace Cielo.Requests
{
    public class CreateTransactionRequest : CieloRequest
    {
        private readonly PaymentMethod _paymentMethod;
        private readonly CreateTransactionOptions _options;
        private readonly Order _order;
        public Guid UniqueKey { get; set; }

        public CreateTransactionRequest(
                Order order,
                PaymentMethod paymentMethod,
                CreateTransactionOptions options,
                IConfiguration configuration = null) : base(configuration)
        {
            _paymentMethod = paymentMethod;
            _options = options;
            _order = order;

            UniqueKey = Guid.NewGuid();
        }

        public override string ToXml(bool indent)
        {
            dynamic xml = new Xml { UseDashInsteadUnderscore = true };
            xml.Declaration(encoding: "ISO-8859-1");
            xml.requisicao_transacao(new { id = UniqueKey, versao = "1.3.0" }, Xml.Fragment(req =>
            {
                Affiliate.ToXml(req, Configuration);

                _order.ToXml(req, Configuration);

                _paymentMethod.ToXml(req);

                req.url_retorno(Configuration.ReturnUrl);
                req.autorizar((int)_options.AuthorizationType);
                req.capturar(_options.Capture.ToString(CultureInfo.InvariantCulture).ToLower());
                req.gerar_token(_options.GenerateToken.ToString(CultureInfo.InvariantCulture).ToLower());
            }));

            return xml.ToString(indent);
        }
    }
}
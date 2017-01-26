using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;
using System.Diagnostics;
using Takenet.MessagingHub.Client.Extensions.Directory;
using Lime.Messaging.Resources;
using PratiqueBot.ContentResolver;
using Lime.Messaging.Contents;
using PratiqueBot.Factory;
using PratiqueBot.Models;
using System.Collections.Generic;
using Takenet.MessagingHub.Client.Extensions.Bucket;
using System.Text.RegularExpressions;
using System.Linq;

namespace PratiqueBot.Receivers
{
    class SportsReceivers : BaseReceiver, IMessageReceiver
    {


        public SportsReceivers(IMessagingHubSender sender, IDirectoryExtension directory, IBucketExtension bucket, Settings settings) : base(sender, directory, bucket, settings)
        {
         
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            string input = message.Content.ToString();


            Trace.TraceInformation($"From: {message.From} \tContent: {message.Content}");
            if (await IsBotActive(message.From))
            {
                Account account = await _directory.GetDirectoryAccountAsync(message.From.ToIdentity(), cancellationToken);

                if (input.Contains("#modalidades#"))
                {
                    await Start(account, message.From, _settings.Sports, cancellationToken);

                }
                else
                {
                    Modality modality = InputToModality(input);
                    string text = GetModalityText(modality);
                    await SendModalityText(text, message.From, cancellationToken);
                    cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
                    await _sender.SendMessageAsync(GetModalityWebLink(modality), message.From, cancellationToken);
                    cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
                    var carrossel = CreateCarrossel(GetGymForModality(modality, _settings.Gyms));
                    await _sender.SendMessageAsync(string.Format("As unidades que possuem a modalidade {0} são :", GetModalityName(modality)), message.From, cancellationToken);
                    await _sender.SendMessageAsync(carrossel, message.From, cancellationToken);
                    await _sender.SendMessageAsync("Os preços podem variar de unidade para unidade, portanto sugiro entrar em contato e falar com nossos atendentes. ", message.From, cancellationToken);
                    await CanIHelpYou(account, message.From, cancellationToken);
                }
            }
        }

        public async Task SendModalityText(string text, Node node, CancellationToken cancellationToken)
        {
            while (text.Length > 320)
            {
                string subText = text.Substring(0, 320);
                text = text.Remove(0, 320);
                await _sender.SendMessageAsync(subText, node, cancellationToken);
                cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            }
            await _sender.SendMessageAsync(text, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));


        }

        public async Task Start(Account account, Node node, List<string> modalities, CancellationToken cancellationToken)
        {
            string modelList = "\n\n";
            foreach (string model in modalities)
            {
                modelList = modelList + "✅" + model + "\n";
            }
            string initialText = string.Format("{0}, atualmente temos as seguintes modalidades: {1}", account.FullName.Split(' ')[0], modelList);
            await _sender.SendMessageAsync(initialText, node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            await _sender.SendMessageAsync("Você pode me perguntar sobre qualquer modalidade quando quiser,\n basta me dizer o nome dela que eu lhe conto mais sobre ela e em quais unidades você pode fazê-la. 😉 ", node, cancellationToken);
            cancellationToken.WaitHandle.WaitOne(new TimeSpan(0, 0, 3));
            Select select = new Select { Text = "Se quiser voltar para o começo, clique abaixo: ", Options = new SelectOption[] { new SelectOption { Text = "Voltar", Value = "#comecar#" } } };
            await _sender.SendMessageAsync(select, node, cancellationToken);
        }

        public string GetModalityText(Modality modality)
        {
            switch (modality)
            {
                case Modality.BOXE:
                    return "As aulas de Boxe além de aumentar o condicionamento físico, ganho de força e equilíbrio, proporciona ao aluno técnicas de defesa pessoal.O Boxe trabalha o corpo e a mente. É uma atividade completa.";
                case Modality.FOURFIT:
                    return "O Four Fit foi desenvolvido pela Academia Pratique Fitness para atender de forma personalizada os alunos (associados) que querem fazer um treinamento mais efetivo. O aluno será acompanhado por uma equipe: Educador Físico, Nutricionista e Avaliador, assim potencializará seus resultados.";
                case Modality.HIDROGINASTICA:
                    return "È uma ginástica realizada dentro da água que intensifica o trabalho de resistência muscular e cardiorrespiratório. Dentro da água o nosso peso é menor, o que reduz o impacto sobre as articulações do corpo.";
                case Modality.JIUJITSU:
                    return "Método de luta desenvolvido no Japão por volta do sXVI, que envolve técnicas de bater, dar pontapés, joelhadas, fazer estrangulamentos e imobilizações, junto com o uso de partes duras do corpo contra pontos vulneráveis do antagonista.";
                case Modality.MUAYTHAY:
                    return "É conhecida mundialmente como a arte das oito armas, pois se caracteriza pelo uso combinado dos dois punhos + dois cotovelos + dois joelhos + dois ‘canelas e pés’, e associado a uma forte preparação física que a torna uma luta de contato total poderosa e eficiente. O muay thai vem ganhando cada vez mais praticantes, é uma luta que desenvolve um ótimo condicionamento físico e mental, concentração e auto-confiança. Além disso, o treinamento ajuda as crianças e adolecentes a terem maior poder de concentração nas suas atividades paralelas.";
                case Modality.MUSCULAÇÃO:
                    return "A musculação ou treinamento com pesos é um tipo de exercício resistido, com variáveis de carga, amplitude, tempo de contração e velocidade controláveis. ";
                case Modality.NATAÇÃO:
                    return "A natação é um esporte que tem ligação direta com uma boa a saúde. È uma atividade indicada para pessoas asmáticas ou com bronquite. Contribui para o fortalecimento muscular, melhora a coordenação dos movimentos e a resistência cardiorrespiratória.";
                case Modality.PERSONAL:
                    return "O Personal Class foi desenvolvido pela Academia Pratique Fitness para atender de forma personalizada os alunos (associados) que querem fazer um treinamento mais efetivo. O aluno será acompanhado por uma equipe multidisciplinar: Educador Físico, Nutricionista e Fisioterapeuta e assim potencializará seus resultados.";
                case Modality.PILATES:
                    return "A cada dia o pilates ganha mais adeptos para a prática dos “Exercicios Pilates”, entre eles, estão muitos artistas que precisam manter uma boa forma e exibir uma excelente postura a seu público. Hoje muitas pessoas já sabem dos beneficios de praticar pilates, é uma técnica de exercícios não aeróbicos, suaves, mas que fornece tônus e fortalece os músculos de dentro pra fora.\n\n";
                case Modality.RITMOS:
                    return "Ritmos é uma aula de Dança + Ginástica com mistura de ritmos quentes, hits do momento, que proporciona grande queima de caloria por aula. Além de aprender a dançar, o aluno perde peso e define o seu corpo.";
                case Modality.SPINNING:
                    return "O Spinning melhora o sistema cardiorrespiratório, as taxas glicêmicas (glicose sanguínea) e o colesterol (HDL e LDL). Proporciona aumento da massa muscular das pernas e bumbum, rápida queima de calorias com redução no percentual de gordura, melhoraa a auto-estima e combate o stress.";
                case Modality.TAEKWONDO:
                    return "O taekwondo foi introduzido em Portugal em 1974 pela mão do Grão-Mestre Chung Sun Yong, atualmente 9ºDAN. A sua introdução teve lugar no Sporting Clube de Portugal, o primeiro Dojang de Taekwondo em Portugal. Em 1978 foram formados os primeiros cintos negros portugueses, tendo desde então sido formados em Portugal um número considerável de Cintos Negros. O fundador de TKD Português continuou a desenvolver a arte marcial não só em Portugal, mas também noutros países como os PALOPs, Israel, antiga URSS, etc";
                case Modality.ZUMBA:
                    return "Zumba é uma aula de Dança com mistura de ritmos quentes, principalmente latinos, que proporciona grande queima de caloria por aula. Além de aprender a dançar, o aluno perde peso e define o seu corpo.";
                default:
                    return "";
            }
        }

        public WebLink GetModalityWebLink(Modality modality)
        {
            switch (modality)
            {
                case Modality.BOXE:
                    return new WebLink { Title = string.Format("Aula de {0}", GetModalityName(modality)), Text = "Unidade Sagrada Familia", Uri = new Uri("https://www.youtube.com/watch?v=l2xOyClwUEE") };
                case Modality.FOURFIT:
                    return new WebLink { Title = string.Format("Aula de {0}", GetModalityName(modality)), Text = "Unidade São Benedito!", Uri = new Uri("https://www.facebook.com/pratiquefitness/videos/1114784335262016/") };
                case Modality.HIDROGINASTICA:
                    return new WebLink { Title = string.Format("Aula de {0}", GetModalityName(modality)), Text = "Unidade Guarani", Uri = new Uri("https://www.facebook.com/pratiquefitness/videos/1060616827345434/") };
                case Modality.JIUJITSU:
                    return new WebLink { Title = string.Format("Aula de {0}", GetModalityName(modality)), Text = "Video explicativo", Uri = new Uri("https://www.youtube.com/watch?v=Eu1VC0ChNrM") };
                case Modality.MUAYTHAY:
                    return new WebLink { Title = string.Format("Aula de {0}", GetModalityName(modality)), Text = "MuayThai", Uri = new Uri("https://www.facebook.com/pratiquefitness/videos/1129919770415139/") };
                case Modality.MUSCULAÇÃO:
                    return new WebLink { Title = string.Format("Aula de {0}", GetModalityName(modality)), Text = "Triceps Kickback Cross Bilateral", Uri = new Uri("https://www.youtube.com/watch?v=YlJY_vEigTE") };
                case Modality.NATAÇÃO:
                    return new WebLink { Title = string.Format("Aula de {0}", GetModalityName(modality)), Text = "Unidade Cachoeirinha", Uri = new Uri("https://www.youtube.com/watch?v=lTrrsCyx3Os") };
                case Modality.PERSONAL:
                    return new WebLink { Title = string.Format("Aula de {0}", GetModalityName(modality)), Text = "Unidade Cachoeirinha", Uri = new Uri("https://www.youtube.com/watch?v=e8l-AljuJGw") };
                case Modality.PILATES:
                    return new WebLink { Title = string.Format("Aula de {0}", GetModalityName(modality)), Text = "Unidade Guarani", Uri = new Uri("https://www.facebook.com/pratiquefitness/videos/1217999351607180/") };
                case Modality.RITMOS:
                    return new WebLink { Title = string.Format("Aula de {0}", GetModalityName(modality)), Text = "Unidade Cachoeirinha", Uri = new Uri("https://www.youtube.com/watch?v=34dqBpkPoLY") };
                case Modality.SPINNING:
                    return new WebLink { Title = string.Format("Aula de {0}", GetModalityName(modality)), Text = "Unidade Cachoeirinha", Uri = new Uri("https://www.youtube.com/watch?v=wRalPPz1wn8") };
                case Modality.TAEKWONDO:
                    return new WebLink { Title = string.Format("Aula de {0}", GetModalityName(modality)), Text = "Taekwondo - Mestre Adolfo Mac Claude", Uri = new Uri("https://www.facebook.com/pratiquefitness/videos/1194157870657995/") };
                case Modality.ZUMBA:
                    return new WebLink { Title = string.Format("Aula de {0}", GetModalityName(modality)), Text = "Unidade Guarani com o professor Tunico", Uri = new Uri("https://www.youtube.com/watch?v=LHFFjSLfMNs") };
                default:
                    return null;
            }
        }

        public List<Gym> GetGymForModality(Modality modality, List<Gym> gyms)
        {
            string modalityName = GetModalityName(modality);
            List<Gym> selectedGyms = new List<Gym>();
            foreach (Gym gym in gyms)
            {
                if (gym.Modalities.Where(a => a == modalityName).Any<string>())
                {
                    selectedGyms.Add(gym);
                }
            }
            return selectedGyms;
        }

        public Document CreateCarrossel(List<Gym> gyms)
        {
            List<CarrosselCard> cards = new List<CarrosselCard>();
            foreach (Gym gym in gyms)
            {
                cards.Add(
                new CarrosselCard
                {
                    CardContent = string.Format("📍:{0} ☎️️: {1}", gym.Address, gym.Phone),
                    CardMediaHeader = new MediaLink
                    {
                        Text = string.Format("📍:{0} ☎️️: {1}", gym.Address, gym.Phone),
                        Uri = new Uri(string.Format("https://maps.googleapis.com/maps/api/staticmap?center={0},{1}&markers=color:red%7Clabel:C%7C{0},{1}&zoom=15&size=600x300&maptype=roadmap&key=AIzaSyAj0zH0MFBnL5oBpUt-SXeSgyCuoLi2caw", gym.latitude, gym.longitude)),
                        Title = gym.Name,
                        Type = new MediaType("image", "jpeg")
                    },
                    options = new List<CarrosselOptions>() {
                        new CarrosselOptions {
                            label = new WebLink {
                                Title = "Ligar Agora",
                                Uri = new Uri(string.Format("tel:+55{0}", gym.Phone).Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", ""))
                            },
                            value = "" },
                        new CarrosselOptions {
                         label= new WebLink
                            {
                                Title = "Traçar Rota",
                                Uri = new Uri(string.Format("https://www.google.com.br/maps/dir/{0},{1}",gym.latitude,gym.longitude))
                            }
                         ,value = ""
                        }
                    }
                });

            }
            var carrossel = _service.CreateCarrossel(cards);
            return carrossel;
        }



        public string GetModalityName(Modality modality)
        {
            switch (modality)
            {
                case Modality.BOXE:
                    return "Boxe";
                case Modality.FOURFIT:
                    return "Four Fit";
                case Modality.HIDROGINASTICA:
                    return "Hidroginástica";
                case Modality.JIUJITSU:
                    return "Jiu Jitsu";
                case Modality.MUAYTHAY:
                    return "Muay-Thai";
                case Modality.MUSCULAÇÃO:
                    return "Musculação";
                case Modality.NATAÇÃO:
                    return "Natação";
                case Modality.PERSONAL:
                    return "Personal";
                case Modality.PILATES:
                    return "Pilates";
                case Modality.RITMOS:
                    return "Ritmo";
                case Modality.SPINNING:
                    return "Spinning";
                case Modality.TAEKWONDO:
                    return "Taekwon Do";
                case Modality.ZUMBA:
                    return "Zumba";
                default:
                    return "";
            }
        }

        public Modality InputToModality(string input)
        {
            Modality modality = new Modality();
            if (new Regex("(!?)(Muay|muay|Muai|muai|Thai|Thay|thai|thay|boxe tailandês|boxe tailandes|Boxe Tailandês|boxe Tailandês|Boxe tailandês|Boxe Tailandes|boxe Tailandes|Boxe tailandes|Luta Tailandeza|luta Tailandeza|Luta tailandeza|luta tailandeza|boxe thailandês|boxe thailandes|Boxe Thailandês|boxe Thailandês|Boxe thailandês|Boxe Thailandes|boxe Thailandes|Boxe thailandes|Luta Thailandeza|luta Thailandeza|Luta thailandeza|luta thailandeza)").IsMatch(input))
            {
                modality = Modality.MUAYTHAY;
            }
            if (new Regex("(!?)(Boxe|boxe)").IsMatch(input))
            {
                modality = Modality.BOXE;
            }
            if (new Regex("(!?)(Ritmos|ritmos|axé|axe|Axé|Axe|coreografias|coreografia|Coreografias|Coreografia)").IsMatch(input))
            {
                modality = Modality.RITMOS;
            }
            if (new Regex("(!?)(Zumba|zumba)").IsMatch(input))
            {
                modality = Modality.ZUMBA;
            }
            if (new Regex("(!?)(Natação|natação|natacão|nataçao|natacao|Natacão|Nataçao|Natacao)").IsMatch(input))
            {
                modality = Modality.NATAÇÃO;
            }
            if (new Regex("(!?)(Musculação|Musculacao|Musculaçao|Musculacão|musculação|musculacao|musculaçao|musculacão|Malhar|malhar|Malhação|malhação|malhacão|malhacao|puxar ferro|Puxar Ferro|Puxar ferro|puxar Ferro)").IsMatch(input))
            {
                modality = Modality.MUSCULAÇÃO;
            }
            if (new Regex("(!?)(Spinning|spinning|spining|Spining|bicicleta|Bicicleta|bicicreta|bike)").IsMatch(input))
            {
                modality = Modality.SPINNING;

            }
            if (new Regex("(!?)(Hidroginastica|Hidroginástica|hidro|Hidro|ginástica na piscina)").IsMatch(input))
            {
                modality = Modality.HIDROGINASTICA;
            }
            if (new Regex("(!?)(Taekw.*|taekw.*)").IsMatch(input))
            {
                modality = Modality.TAEKWONDO;
            }
            if (new Regex("(!?)(Jiu.*|jiu.*|.*Jitsu|.*jitsu)").IsMatch(input))
            {
                modality = Modality.JIUJITSU;
            }
            if (new Regex("(!?)(Personal|personal)").IsMatch(input))
            {
                modality = Modality.PERSONAL;
            }
            if (new Regex("(!?)(FourFit|fourFit|fourfit|forFit|Forfit|ForFit)").IsMatch(input))
            {
                modality = Modality.FOURFIT;
            }

            return modality;
        }

    }

    public enum Modality
    {
        MUAYTHAY,
        BOXE,
        NATAÇÃO,
        RITMOS,
        MUSCULAÇÃO,
        SPINNING,
        PILATES,
        HIDROGINASTICA,
        TAEKWONDO,
        ZUMBA,
        PERSONAL,
        JIUJITSU,
        FOURFIT
    }
}
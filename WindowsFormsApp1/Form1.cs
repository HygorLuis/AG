using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class frmAg : Form
    {
        public frmAg()
        {
            InitializeComponent();
        }

        List<Sorteado> sorteado = new List<Sorteado>();
        List<Individuo> individuo = new List<Individuo>();
        List<Filhos> filhos = new List<Filhos>();
        Random rSorteia = new Random();
        string sObjetivo = "010010010010";
        int iNumIndividuo = 8;
        int iNumGene = 12;
        bool bPerfeito = false;

        private string gerarIndividuo()
        {
            string individuo = "";
            for (int i = 1; i <= iNumGene; i++)
            {
                individuo = individuo + rSorteia.Next(2).ToString();
            }

            return individuo;
        }

        private bool fitness()
        {
            var ObjetivoGenes = sObjetivo.ToArray();
            for (int i = 0; i < iNumIndividuo; i++) //ESCOLHE OS INDIVIDUOS PARA VERIFICAR SEU FITNESS
            {
                int ifitness = 0;
                var individuoGenes = individuo[i].Genes.ToArray();
                for (int x = 0; x < iNumGene; x++)
                {          
                    if (ObjetivoGenes[x].Equals(individuoGenes[x])) //VERIFICA A QUANTIDADE DE GENES PERFEITOS
                        ifitness++;
                }

                individuo[i].Fitness = ifitness;

                if (ifitness == iNumGene)
                    return true;
            }

            return false;
        }

        private void reproduzir()
        {
            int dSoma = 0;
            for (int i = 0; i < iNumIndividuo; i++) //COLOCA OS INDIVIDUOS NA "ROLETA" E GUARDA A POSIÇÃO DE CADA UM
            {
                dSoma += Convert.ToInt32(individuo[i].Fitness);
                individuo[i].Posicao = dSoma;
            }

            sorteado.Clear();
            while (sorteado.Count < iNumIndividuo) //VERIFICA A QUANTIDADE DE INVIDUOS SORTEADOS GRAVADOS NA LISTA
            {
                var posicaoSorteado = rSorteia.Next(dSoma + 1); //DEFINE QUAL POSIÇÃO A "ROLETA PAROU"
                for (int x = 0; x < iNumIndividuo; x++)
                {
                    if (individuo[x].Posicao >= posicaoSorteado) //VERIFICA O INDIVIDUO SORTEADO
                    {
                        //GUARDA NA LISTA OS INDIVIDUOS (GENES) SORTEADOS
                        sorteado.Add(new Sorteado
                        {
                            Genes = individuo[x].Genes
                        });

                        break;
                    }
                }
            }

            filhos.Clear();
            for (int i = 0; i < iNumIndividuo/2; i++)
            {
                var pai = sorteado[i].Genes; //DEFINE PAI
                var mae = sorteado[(individuo.Count-1)-i].Genes; //DEFINE MÃE
                var corte = rSorteia.Next(iNumGene + 1); //POSIÇÃO QUE OS PAIS SERÃO "CORTADOS"

                //if (!pai.Equals(mae))
                //{
                    if (Convert.ToBoolean(rSorteia.Next(2))) //DECIDE SE O FILHO 1 SOFRERÁ MUTAÇÃO
                    {
                        filhos.Add(new Filhos
                        {           //FUNÇÃO QUE IRÁ REALIZAR A MUTAÇÃO
                            Genes = mutar(
                                pai.Substring(0, corte) //GENES DO PAI
                                + mae.Substring(corte, iNumGene - corte), //GENES DA MÃE
                                rSorteia.Next(iNumGene)) //POSIÇÃO DE MUTAÇÃO
                        });
                    }
                    else
                    {
                        filhos.Add(new Filhos
                        {           //GENES DO PAI            //GENES DA MÃE
                            Genes = pai.Substring(0, corte) + mae.Substring(corte, iNumGene - corte)
                        });
                    }



                    if (Convert.ToBoolean(rSorteia.Next(2))) //DECIDE SE O FILHO 2 SOFRERÁ MUTAÇÃO
                    {
                        filhos.Add(new Filhos
                        {           //FUNÇÃO QUE IRÁ REALIZAR A MUTAÇÃO
                            Genes = mutar(
                                mae.Substring(0, corte) //GENES DA MÃE
                                + pai.Substring(corte, iNumGene - corte), //GENIS DO PAI
                                rSorteia.Next(iNumGene)) //POSIÇÃO DE MUTAÇÃO
                        });
                    }
                    else
                    {
                        filhos.Add(new Filhos
                        {           //GENES DA MÃE            //GENES DO PAI
                            Genes = mae.Substring(0, corte) + pai.Substring(corte, iNumGene - corte)
                        });
                    }
                //}
            }
            
        }

        private bool verificar(List<Individuo> analiseIndividuo)
        {
            for (int i = 0; i < analiseIndividuo.Count; i++)
            {
                if (analiseIndividuo[i].Genes.Equals(sObjetivo))
                {
                    MessageBox.Show("Individuo Perfeito.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }                   
            }
            return false;
        }

        private string mutar (string filho, int posicaoMutacao)
        {
            var mutacao = filho.Substring(posicaoMutacao, 1) == "1" ? "0" : "1"; //REALIZA MUTAÇÃO DO GENE

            var aStringBuilder = new StringBuilder(filho);
            aStringBuilder.Remove(posicaoMutacao, 1); //REMOVE O "GENE" DA POSIÇÃO ANTES DE SER MUTADO 
            aStringBuilder.Insert(posicaoMutacao, mutacao); //INSERE O "GENE" MUTADO NA POSIÇÃO
            return mutacao = aStringBuilder.ToString();
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            individuo.Clear();
            bPerfeito = false;

            listBox1.Items.Clear();
            listBox1.Items.Add("1°");
            listBox1.Items.Add("===============");
            listBox1.Items.Add("===INDIVIDUOS===");
            for (int i = 0; i < iNumIndividuo; i++)
            {
                //GUARDA NA LISTA OS INDIVIDUOS GERADOS
                individuo.Add(
                    new Individuo
                    {           //GERA OS PRIMEIROS INDIVIDUOS
                        Genes = gerarIndividuo()
                    });
                listBox1.Items.Add("    " + individuo[i].Genes);
                listBox1.Update();
            }
            listBox1.Items.Add("===============");
            listBox1.Items.Add("===============");

            //CASO A PRIMEIRA GERAÇÃO DE FILHOS NÃO CHEGUE A PERFEIÇÃO, CONTINUA A REPRODUÇÃO ENTRE OS MESMOS
            for (int iCount = 2; iCount < 500; iCount++)
            {
                if (bPerfeito)
                    break;

                //AVALIA O FITNESS DE CADA INDIVIDUO
                if (fitness())
                {
                    MessageBox.Show("Individuo Perfeito.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    bPerfeito = true;
                    continue;
                }

                reproduzir();

                listBox1.Items.Add(iCount.ToString() + "°");
                listBox1.Items.Add("===============");
                listBox1.Items.Add("===INDIVIDUOS===");

                //VERIFICA SE ALGUM FILHO CHEGOU A PERFEIÇÃO
                for (int i = 0; i < filhos.Count; i++) 
                {
                    listBox1.Items.Add("    " + filhos[i].Genes);
                    listBox1.Update();

                    if (filhos[i].Genes.Equals(sObjetivo))
                    {
                        MessageBox.Show("Individuo Perfeito.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        bPerfeito = true;
                        listBox1.Items.Add("===============");
                        listBox1.Items.Add("===============");
                        break;
                    }
                }
                

                if (!bPerfeito)
                {
                    listBox1.Items.Add("===============");
                    listBox1.Items.Add("===============");
                    individuo.Clear(); //"MATA" OS PAIS
                    foreach (var novoIndividuo in filhos) //OS FILHOS ASSUMEM OS LUGARES DOS PAIS
                    {
                        individuo.Add(new Individuo
                        {
                            Genes = novoIndividuo.Genes
                        });
                    }
                    //filhos.Clear();
                }
            }

            if (!bPerfeito)
                MessageBox.Show("Individuo Perfeito.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

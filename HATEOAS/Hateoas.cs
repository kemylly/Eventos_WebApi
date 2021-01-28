using System.Collections.Generic;

namespace treino_api.HATEOAS
{
    public class Hateoas
    {
        private string url; //url para chegar até o controller
        private string protocol = "https://";
        public List<Link> actions = new List<Link>();  //acoes na lista que vem do controller
        private string defaultUrl;
        
         public Hateoas(string url)
        {
            this.url = url;
        }

        public Hateoas(string url, string protocol)
        {
            this.url = url;
            this.protocol = protocol;
            defaultUrl = this.protocol + this.url;
        }
        public void AddAction(string rel, string method)
        {
            actions.Add(new Link(this.protocol + this.url,rel,method));
        }

        public Link[] GetActions(string sufix)
        {
            Link[] tempLinks = new Link[actions.Count];
            
            for(int i=0; i < tempLinks.Length;i++)
            {
                tempLinks[i] = new Link(actions[i].href, actions[i].rel, actions[i].method);
            }

            /* montagem do link */
            foreach(var link in tempLinks){
                // https:// localhost:5001/api/v1/Eventos/ 2/32/kemylly
                link.href = link.href+"/"+sufix;
            }
            return tempLinks;
        }
    }
}